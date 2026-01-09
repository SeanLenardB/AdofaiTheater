using System.Diagnostics;
using AdofaiTheater.Foundation.Basic;
using AdofaiTheater.Foundation.Core;
using AdofaiTheater.Foundation.Timeline;

namespace AdofaiTheater
{
    public class Program
    {
        private static void Main()
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();  // element instantiation

            Theater theater = new();
            theater.Configuration.OutputPath = "output";
            theater.AddElement(
                new TheaterImage()
                { ImagePath = @"Resources/ori.png" }
                .AsBackground(theater, TheaterImage.BackgroundScalingPolicy.FILL_SCREEN));
			TheaterImage moveableImage = new() { ImagePath = @"Resources/quartrond.png" };
            theater.AddElement(moveableImage);

            TheaterCharacter characterQuartrond = new();
            theater.AddElement(characterQuartrond);

            theater.PushEvent(new TheaterElementParameterizedAnimation(20, 
                _ =>
                {
                    moveableImage.Transform.Move(100, 0);
                }));

            stopwatch.Stop();   // element instantiation
			Console.WriteLine($"Instantiation time: {stopwatch.Elapsed.TotalSeconds}s.");

            stopwatch.Start();  // theater rendering
            theater.Animate();
            stopwatch.Stop();   // theater rendering
			Console.WriteLine($"Render time:        {stopwatch.Elapsed.TotalSeconds}s.");
        }
    }
}