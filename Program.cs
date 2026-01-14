using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using AdofaiTheater.Compiler;
using AdofaiTheater.Foundation.Basic;
using AdofaiTheater.Foundation.Core;
using AdofaiTheater.Foundation.Timeline;

namespace AdofaiTheater
{
    [SupportedOSPlatform("windows")]
    public class Program
    {
        private static void Main()
        {
            TheaterCompiler compiler = new();

            Stopwatch stopwatch = new();
            stopwatch.Start();    // element instantiation

            compiler.Theater.Configuration.OutputPath = "output";
            compiler.AddElement("bg",
                new TheaterImage()
                { ImagePath = @"Resources/journeyend.png" }
                .AsBackground(compiler.Theater, TheaterImage.BackgroundScalingPolicy.FILL_SCREEN));

            TheaterImage imageTrack = new() { ImagePath = @"Resources/adofaitrack.png" };
            imageTrack.PivotAtCenter();
            compiler.AddElement("move", imageTrack);

            TheaterImage imageOneattempt = new() { ImagePath = @"Resources/oneattempt.png" };
            imageOneattempt.PivotAtCenter();
            imageOneattempt.Transform.PositionSet(960, 480);
            imageOneattempt.Transform.Layer = 10;
            compiler.AddElement("meta", imageOneattempt);

            compiler.AppendSpeechAndSubtitle("A simple text to test the text rendering.");
            compiler.AttachEventAutoDuration(new TheaterElementParameterizedAnimation(t =>
            {
                imageTrack.Transform.ScaleAdd(t / 10);
            }));

            compiler.AppendSpeechAndSubtitle("你的脸怎么红了？容光焕发！");
            compiler.AttachEventAutoDuration(new TheaterElementParameterizedAnimation(t =>
            {
                imageTrack.Transform.PositionSet(100 + (1500 * t), 300 * t);
                imageOneattempt.Transform.RotationSet(720 * t);
            }).WithEase(new InSineParameterizedEase()));

            compiler.AppendSpeechAndSubtitle("你的脸，怎么又黄了？我脸黄不黄跟你有关系吗");
            compiler.AttachEvent(new TheaterElementParameterizedAnimation(0, _ =>
            {
                imageOneattempt.Transform.Layer = -10;
            }));
            compiler.AttachEventAutoDuration(new TheaterElementParameterizedAnimation(t =>
            {
                imageTrack.Transform.ScaleMultiply(0.95, 0.95);
                imageOneattempt.Transform.RotationSet(-720 * t);
            }));
            compiler.AttachEventAutoDuration(new TheaterElementParameterizedAnimation(t =>
            {
                imageTrack.Transform.PositionSet(1600, 300 + (1000 * t));
            }).WithEase(new OutSineParameterizedEase()));

            stopwatch.Stop();     // element instantiation
            Console.WriteLine($"Compilation:   {stopwatch.Elapsed.TotalSeconds}s.");

            stopwatch.Restart();  // theater rendering
            compiler.Compile();
            stopwatch.Stop();     // theater rendering
            Console.WriteLine($"Render:        {stopwatch.Elapsed.TotalSeconds}s.");
        }
    }
}