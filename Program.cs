using AdofaiTheater.Foundation.Drawing;
using AdofaiTheater.Foundation.Theater;

namespace AdofaiTheater
{
    public class Program
    {
        private static void Main()
        {
            Theater mainTheater = new();
            mainTheater.Elements.Add(new TheaterImage() { ImagePath = @"Resources/ori.png" });

            mainTheater.Animate();
        }
    }
}