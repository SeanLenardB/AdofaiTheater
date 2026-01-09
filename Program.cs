using AdofaiTheater.Foundation.Core;
using AdofaiTheater.Foundation.Drawing;

namespace AdofaiTheater
{
    public class Program
    {
        private static void Main()
        {
            Theater theater = new();
            theater.Configuration.OutputPath = "output";
            theater.Add(
                new TheaterImage()
                { ImagePath = @"Resources/ori.png" }
                .AsBackground(theater, TheaterImage.BackgroundScalingPolicy.FILL_SCREEN));

            theater.Animate();
        }
    }
}