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
using AdofaiTheater.Foundation.Prefabs;
using AdofaiTheater.Foundation.Timeline;
using FFMpegCore;
using FFMpegCore.Pipes;
using NAudio.Wave;

namespace AdofaiTheater.Compiler
{
    // TODO(seanlb): implement the script compiler
    [SupportedOSPlatform("windows")]
    public class TheaterCompiler
    {
        public Theater Theater { get; private set; } = new();

        private Dictionary<string, ITheaterElement> Elements { get; set; } = [];

        public TheaterCompiler AddElement<T>(string id, T element) where T : ITheaterElement
        {
            // NOTE(seanlb): This can become a compiler error.
            Debug.Assert(!this.Elements.ContainsKey(id), "Another element with the same id exists!");

            this.Elements.Add(id, element);

            this.Theater.AddElement(element);
            return this;
        }

        // WARN(seanlb): This is probably unnecessary because in the script you need to instantiate an element
        // in order to add it to the theater. There is no need for this function. Please reduce its calls.
        // If in the future, there is such need, this warn will get removed.
        public T GetElement<T>(string id) where T : class, ITheaterElement
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
        private List<TheaterSpeechSegment> Segments { get; set; } = [];

        public TheaterCompiler AppendSpeech(string speech)
        {
            TheaterSpeechSegment segment = 
                TheaterSpeechSynthesizer.Synthesize(speech, 
                    this.Theater.Configuration.ConcatenatePath($"Output_Audio_Segment_{this.Segments.Count}.wav"));
            this.Segments.Add(segment);

            return this;
        }

        public TheaterCompiler AppendSpeechAndSubtitle(string speech)
        {
            this.AppendSpeech(speech);

            TheaterText speechElement = new TheaterText(speech).AsTheaterSubtitle(this.Theater);
            // TODO(seanlb): finish this after optimization
            this.AddElement($"_THEATER_SPEECH_INDEX_{this.Segments.Count - 1}_", speechElement);

            // NOTE(seanlb):
            // If this is the first subtitle, we would want it to be visible at the beginning.
            // But, if this is not, we don't want it to be on the canvas at the beginning.
            // So, it will be NOT visible, and be animated to be visible at the end of the previous speech.
            //
            // Also note that this is a very scuffed way to do OnFrameStart()
            // If in the future, there are more frame start events, we should migrate to a proper way to do it.
            if (this.Segments.Count >= 2)
            {
                speechElement.Transform.Visible = false;
                TheaterElementParameterizedAnimation subtitleVisibleAnimation =
                    new(t =>
                    {
                        if (t == 1d) { speechElement.Transform.Visible = true; }
                    });
                subtitleVisibleAnimation.SetTotalFrames((int)(this.Segments[^2].SpeechDuration.TotalSeconds * this.Theater.Configuration.FramesPerSecond));
                this.Segments[^2].BoundEvents.Add(subtitleVisibleAnimation);
            }

            this.AttachEventAutoDuration(T => new TheaterElementParameterizedAnimation(T, t =>
                {
                    if (t == 1d) { speechElement.Transform.Visible = false; }
                }));

            return this;
        }

        public TheaterCompiler AttachEvent(ITheaterEvent theaterEvent)
        {
            Debug.Assert(this.Segments.Count > 0, "You need to append a speech before attaching events!");
            this.Segments.Last().BoundEvents.Add(theaterEvent);
            return this;
        }

        /// <summary>
        /// <see cref="AttachEvent(ITheaterEvent)"/> but with more flexibility.
        /// The <paramref name="onTotalFramesDetermined"/>'s <see cref="int"/> parameter is the total frames once the audio is generated.
        /// Return the final <see cref="ITheaterEvent"/>.
        /// </summary>
        public TheaterCompiler AttachEventAutoDuration(Func<int, ITheaterEvent> onTotalFramesDetermined)
        {
            Debug.Assert(this.Segments.Count > 0, "You need to append a speech before attaching events!");
            this.AttachEvent(onTotalFramesDetermined((int)(this.Segments.Last().SpeechDuration.TotalSeconds * this.Theater.Configuration.FramesPerSecond)));
            return this;
        }



        public void Compile()
        {
            int segmentIndex = 0;
            var frames = this.Theater.Animate(frame =>
            {
                if (this.Segments
                        .Take(segmentIndex + 1)  // NOTE(seanlb): This is ok because we can take 1000 elements from an empty thing.
                        .Sum(segment => (int)(segment.SpeechDuration.TotalSeconds * this.Theater.Configuration.FramesPerSecond)) < frame)
                { segmentIndex++; }
                if (segmentIndex >= this.Segments.Count) { return false; }

                // NOTE(seanlb): very good trick because the predicate will call the function once.
                this.Segments[segmentIndex].BoundEvents.RemoveAll(e => !e.NextFrame());
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
            for (int i = 0; i < this.Segments.Count; i++)
            {
                ffmpegArguments.AddFileInput(
                    this.Theater.Configuration.ConcatenatePath($"Output_Audio_Segment_{i}.wav"),
                    true,
                    options => options
                        .WithFramerate(this.Theater.Configuration.FramesPerSecond)
                        .Seek(TimeSpan.Zero)
                        .WithDuration(this.Segments[i].SpeechDuration));
                concatArgument += $"[{i + 1}:a]";
            }
            concatArgument += $"concat=n={this.Segments.Count}:v=0:a=1[speechconcat]";

            bool success = ffmpegArguments
                .OutputToFile(
                "Final.mp4", true,
                options => options
                    .ForceFormat("mp4")
                    .ForcePixelFormat("yuv420p")
                    .WithAudioBitrate(128)
                    .WithFramerate(this.Theater.Configuration.FramesPerSecond)
                    .WithCustomArgument($"-filter_complex \"{concatArgument}\"")
                    .WithCustomArgument("-map 0:v")
                    .WithCustomArgument("-map [speechconcat]")
                    .WithCustomArgument("-movflags +faststart")
                    .WithVideoCodec("libx264")
                    .WithAudioCodec("aac"))
                .ProcessSynchronously();
            Debug.Assert(success, "FFmpeg muxing failed!");
        }
    }

    public class TheaterSpeechSegment
    {
        public TimeSpan SpeechDuration { get; set; } = TimeSpan.Zero;
        public string SpeechFileLocation { get; set; } = "";

        public List<ITheaterEvent> BoundEvents { get; set; } = [];
    }
}
