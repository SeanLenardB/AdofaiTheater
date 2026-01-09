using AdofaiTheater.Foundation.Drawing;
using AdofaiTheater.Foundation.Theater;

namespace AdofaiTheater
{
    public class Program
    {
        private static void Main()
        {
            Theater theater = new();
            theater.Configuration.OutputPath = "output";
            theater.Elements.Add(new TheaterImage() { ImagePath = @"Resources/ori.png" });

            theater.Animate();
        }
    }
}