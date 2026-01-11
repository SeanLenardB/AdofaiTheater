using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using AdofaiTheater.Compiler;
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
                { ImagePath = @"Resources/adventbg.png" }
                .AsBackground(compiler.Theater, TheaterImage.BackgroundScalingPolicy.FILL_SCREEN));

            TheaterImage moveableImage = new() { ImagePath = @"Resources/twirl.png" };
            moveableImage.PivotAtCenter();
            moveableImage.Transform.PositionAdd(100, 50);
            compiler.AddElement("move", moveableImage);

            compiler.AppendSpeech("你的脸怎么红了？");

            compiler.AppendSpeech("容光焕发！");
            compiler.AttachEventAutoDuration(new TheaterElementParameterizedAnimation(t =>
            {
                moveableImage.Transform.PositionAdd(30, t * t * 50);
            }));

            compiler.AppendSpeech("你的脸，怎么又黄了？");
            compiler.AttachEventAutoDuration(new TheaterElementParameterizedAnimation(t =>
            {
                moveableImage.Transform.RotateClockwise(50 - (50 * t * t * t));
            }));

            compiler.AppendSpeech("我脸黄不黄跟你有关系吗");
            compiler.AttachEventAutoDuration(new TheaterElementParameterizedAnimation(t =>
            {
                moveableImage.Transform.ScaleMultiply(0.98, 0.98);
                moveableImage.Transform.PositionAdd(t * t * t * t * t * 400, 0);
            }));

            stopwatch.Stop();     // element instantiation
            Console.WriteLine($"Compilation:   {stopwatch.Elapsed.TotalSeconds}s.");

            stopwatch.Restart();  // theater rendering
            compiler.Compile();
            stopwatch.Stop();     // theater rendering
            Console.WriteLine($"Render:        {stopwatch.Elapsed.TotalSeconds}s.");
        }
    }
}