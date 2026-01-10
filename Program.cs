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
            // TODO(seanlb): The transform system is a little bit broken. Remember to fix it before moving on.

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
            moveableImage.Transform.Move(-100, 0);
            compiler.AddElement("move", moveableImage);

            compiler.AppendSpeech("Hi!");

            compiler.AppendSpeech("This is a video synthesized through programming.");
            compiler.AttachEventAutoDuration(new TheaterElementParameterizedAnimation(t =>
            {
                moveableImage.Transform.Move(10, t * t * 20);
            }));

            compiler.AppendSpeech("It's my side project and it's very poorly written.");
            compiler.AttachEventAutoDuration(new TheaterElementParameterizedAnimation(t =>
            {
                moveableImage.Transform.Rotate(50 - (50 * t * t * t));
            }));

            compiler.AppendSpeech("I'm just experimenting with f f m peg.");
            compiler.AttachEventAutoDuration(new TheaterElementParameterizedAnimation(t =>
            {
                moveableImage.Transform.Scale(0.98, 0.98);
                moveableImage.Transform.Move(t * t * t * t * t * 400, 0);
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