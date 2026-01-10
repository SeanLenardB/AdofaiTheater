using System.Diagnostics;
using AdofaiTheater.Compiler;
using AdofaiTheater.Foundation.Basic;
using AdofaiTheater.Foundation.Core;
using AdofaiTheater.Foundation.Timeline;

namespace AdofaiTheater
{
    public class Program
    {
        private static void Main()
        {
            TheaterCompiler compiler = new();

            Stopwatch stopwatch = new();
            stopwatch.Start();  // element instantiation

            compiler.Theater.Configuration.OutputPath = "output";
            compiler.AddElement("bg",
                new TheaterImage()
                { ImagePath = @"Resources/ori.png" }
                .AsBackground(compiler.Theater, TheaterImage.BackgroundScalingPolicy.FILL_SCREEN));

			TheaterImage moveableImage = new() { ImagePath = @"Resources/quartrond.png" };
            moveableImage.Transform.SetPivot(800, 200);
            compiler.AddElement("move", moveableImage);

            stopwatch.Stop();   // element instantiation
			Console.WriteLine($"Compilation:   {stopwatch.Elapsed.TotalSeconds}s.");

            stopwatch.Start();  // theater rendering
            compiler.Theater.Animate();
            stopwatch.Stop();   // theater rendering
			Console.WriteLine($"Render:        {stopwatch.Elapsed.TotalSeconds}s.");

            // TODO(seanlb): implement ffmpeg step
        }
    }
}