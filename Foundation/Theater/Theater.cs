using AdofaiTheater.Foundation.Drawing;
using SkiaSharp;

namespace AdofaiTheater.Foundation.Theater
{
    public class Theater
    {
        public Theater() { }



        public TheaterConfiguration Configuration { get; set; } = new();



        public List<TheaterElement> Elements { get; private set; } = [];
        public void Animate()
        {
            using (SKSurface surface = SKSurface.Create(new SKImageInfo(this.Configuration.Width, this.Configuration.Height)))
            {
                foreach (var element in this.Elements.OrderByDescending(e => e.Layer)) { element.Draw(surface.Canvas); }

                using (SKData imageData = surface.Snapshot().Encode(SKEncodedImageFormat.Png, this.Configuration.ImageQuality))
                {
                    imageData.SaveTo(File.OpenWrite("output.png"));
                }
            }
        }
    }

    public class TheaterConfiguration
    {
        public int Width { get; set; } = 1920;
        public int Height { get; set; } = 1080;
        // NOTE(seanlb): I have no idea what this is
        public int ImageQuality { get; set; } = 100;
    }
}
