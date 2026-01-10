using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Versioning;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using AdofaiTheater.Foundation.Basic;
using AdofaiTheater.Foundation.Core;
using AdofaiTheater.Foundation.Timeline;
using FFMpegCore;
using FFMpegCore.Pipes;

namespace AdofaiTheater.Compiler
{
    // TODO(seanlb): implement the script compiler
    public class TheaterCompiler
    {
        public Theater Theater { get; private set; } = new();

        private Dictionary<string, TheaterElement> Elements { get; set; } = [];

        public TheaterCompiler AddElement<T>(string id, T element) where T : TheaterElement
        {
            // NOTE(seanlb): This can become a compiler error.
            Debug.Assert(!this.Elements.ContainsKey(id), "Another element with the same id exists!");

            this.Elements.Add(id, element);

            this.Theater.AddElement(element);
            return this;
        }

        public T GetElement<T>(string id) where T : TheaterElement
        {
            // NOTE(seanlb): This can become a compiler error.
            // Also, I don't want a nullable type here.
            Debug.Assert(!this.Elements.ContainsKey(id), "The id does not exist!");
            Debug.Assert(this.Elements[id] is T, "The target element is not an instance of the generic type!");

            return (this.Elements[id] as T)!;
        }



        // NOTE(seanlb): A summary of the model:
        // 1. Speeches dominate the flow of the theater. That is, the timeline is driven by the speech.
        // 2. Each speech may be accompanied by some animations.
        //    (The animations are aligned to the start of the speech. To attach an event at the middle of the speech,
        //     split the speech into two segments and attach the event to the second one.)
        // 3. Empty speech is used for pauses.
        //
        // Steps to call the methods:
        // 1. this.AppendSpeech(...);
        // 2. this.AttachEvent(...);
        // 3. repeat the steps until the theater is done.
        // 4. this.Theater.Animate();  // NOTE(seanlb): This might be changed to this.Compile();
        private Queue<TheaterSpeechSegment> Segments { get; set; } = [];
        [SupportedOSPlatform("windows")]
        public TheaterCompiler AppendSpeech(string speech)
        {
            TheaterSpeechSegment segment = new()
            {
                SpeechFileLocation = this.Theater.Configuration.ConcatenatePath($"Output_Audio_Segment_{this.Segments.Count}.wav")
            };
            this.Segments.Enqueue(segment);

            PromptBuilder builder = new(new("zh-CN"));
            builder.AppendText(speech);
            builder.AppendBookmark("_SPEECH_END_");

            using (SpeechSynthesizer synthesizer = new())
            {
                synthesizer.SelectVoice("Microsoft Kangkang");
                synthesizer.Rate = 5;

                synthesizer.BookmarkReached += (o, e) =>
                {
                    if (e.Bookmark != "_SPEECH_END_") { return; }  // NOTE(seanlb): For safety.
                    segment.SpeechDurationFrames = (int)(e.AudioPosition.TotalSeconds * this.Theater.Configuration.FramesPerSecond);
                };

                synthesizer.SetOutputToWaveFile(segment.SpeechFileLocation);

                synthesizer.Speak(builder);

            }
            return this;
        }

        public TheaterCompiler AttachEvent(ITheaterEvent theaterEvent)
        {
            Debug.Assert(this.Segments.Count > 0, "You need to append a speech before attaching events!");
            this.Segments.Last().BoundEvents.Add(theaterEvent);
            return this;
        }

        public TheaterCompiler AttachEventAutoDuration(ITheaterAdjustableDurationEvent theaterEvent)
        {
            Debug.Assert(this.Segments.Count > 0, "You need to append a speech before attaching events!");
            theaterEvent.SetTotalFrames(this.Segments.Last().SpeechDurationFrames);
            this.Segments.Last().BoundEvents.Add(theaterEvent);
            return this;
        }



        public void Compile()
        {
            int segmentCount = this.Segments.Count;
            TheaterSpeechSegment? currentSegment = null;
            var frames = this.Theater.Animate(t =>
            {
                if (currentSegment is not null && currentSegment.BoundEvents.Count == 0) { currentSegment = null; }
                if (this.Segments.Count == 0 && currentSegment is null) { return false; }

                currentSegment ??= this.Segments.Dequeue();

                // NOTE(seanlb): very good trick because the predicate will call the function once.
                currentSegment.BoundEvents.RemoveAll(e => !e.NextFrame());

                return true;
            });

            Debug.Assert(File.Exists(@"Resources\ffmpeg.exe"), "ffmpeg should be placed under Resources folder.");
            GlobalFFOptions.Configure(options =>
            {
                options.BinaryFolder = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
                options.WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), this.Theater.Configuration.OutputPath);
            });


            RawVideoPipeSource videoSource = new(frames) { FrameRate = this.Theater.Configuration.FramesPerSecond };
            FFMpegArguments ffmpegArguments = FFMpegArguments.FromPipeInput(videoSource);

            // NOTE(seanlb): audio inputs must be added in a loop. Writing a custom pipesource requires parsing .wav files.
            // Also, ffmpeg will only insert the first audio track. To concatenate them, here's some very shit code to do it.
            //
            // Don't worry if you can't understand this. The string we want to achieve is:
            // -filter_complex '[1:0][2:0][3:0]...concat=n=...:v=0:a=1[speechconcat]'               (cli argument)
            // https://trac.ffmpeg.org/wiki/Concatenate
            // 
            // The result is our juicy audio will be ready inside speechconcat pipe.
            string concatArgument = "";
            for (int i = 0; i < segmentCount; i++)
            {
                ffmpegArguments.AddFileInput(this.Theater.Configuration.ConcatenatePath($"Output_Audio_Segment_{i}.wav"));
                concatArgument += $"[{i + 1}:a]";
            }
            concatArgument += $"concat=n={segmentCount}:v=0:a=1[speechconcat]";

            bool success = ffmpegArguments
                .OutputToFile(
                "Final.mp4", true,
                options => options
                    .ForceFormat("mp4")
                    .ForcePixelFormat("yuv420p")
                    .WithCustomArgument($"-filter_complex \"{concatArgument}\"")
                    .WithCustomArgument($"-map 0:v")
                    .WithCustomArgument($"-map [speechconcat]")
                    .WithVideoCodec("libx264")
                    .WithAudioCodec("aac"))
                .ProcessSynchronously();
            Debug.Assert(success, "FFmpeg muxing failed!");
        }
    }

    public class TheaterSpeechSegment
    {
        public int SpeechDurationFrames { get; set; } = 0;
        public string SpeechFileLocation { get; set; } = "";

        public List<ITheaterEvent> BoundEvents { get; set; } = [];
    }
}
