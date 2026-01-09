using AdofaiTheater.Foundation.Basic;
using AdofaiTheater.Foundation.Core;

namespace AdofaiTheater
{
    public class Program
    {
        private static void Main()
        {
            Theater theater = new();
            theater.Configuration.OutputPath = "output";
            theater.AddElement(
                new TheaterImage()
                { ImagePath = @"Resources/ori.png" }
                .AsBackground(theater, TheaterImage.BackgroundScalingPolicy.FILL_SCREEN));

            TheaterCharacter characterQuartrond = new();
            theater.AddElement(characterQuartrond);

            theater.Animate();
        }
    }
}