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
                { ImagePath = @"Resources/ori.png" }
                .AsBackground(compiler.Theater, TheaterImage.BackgroundScalingPolicy.FILL_SCREEN));

            TheaterImage moveableImage = new() { ImagePath = @"Resources/quartrond.png" };
            moveableImage.Transform.SetPivot(800, 200);
            compiler.AddElement("move", moveableImage);

            compiler.AppendSpeech("操");
            compiler.AttachEventAutoDuration(new TheaterElementParameterizedAnimation(t =>
            {
                moveableImage.Transform.Rotate(-t * 10);
            }));

            compiler.AppendSpeech("你是真没见过黑手啊");
            compiler.AttachEventAutoDuration(new TheaterElementParameterizedAnimation(t =>
            {
                moveableImage.Transform.Move(20, -t * 30);
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