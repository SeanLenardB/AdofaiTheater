using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using FFMpegCore;
using FFMpegCore.Pipes;

namespace AdofaiTheater.Compiler
{
    public partial class TheaterCompiler
    {
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
}
