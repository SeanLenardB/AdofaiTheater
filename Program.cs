using AdofaiTheater.Foundation.Basic;
using AdofaiTheater.Foundation.Core;
using AdofaiTheater.Foundation.Timeline;

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
			TheaterImage moveableImage = new() { ImagePath = @"Resources/quartrond.png" };
            theater.AddElement(moveableImage);

            TheaterCharacter characterQuartrond = new();
            theater.AddElement(characterQuartrond);

            theater.PushEvent(new TheaterElementParameterizedAnimation(10, 
                _ =>
                {
                    moveableImage.Transform.Move(100, 0);
                    moveableImage.Transform.Rotate(5);
                }));

            theater.Animate();
        }
    }
}