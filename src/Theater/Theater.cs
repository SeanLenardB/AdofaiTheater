using SkiaSharp;

namespace AdofaiTheater
{
    public class Theater
    {
        public Theater() { }



        public TheaterConfiguration Configuration { get; set; } = new();



        public List<ITheaterElement> Elements { get; private set; } = [];
        public void Animate()
        {
            using SKSurface surface = SKSurface.Create(new SKImageInfo(Configuration.Width, Configuration.Height));

            surface.Canvas.DrawCircle(100, 100, 50, new() { Color = new(0xff, 0xff, 0xff, 0xaf) });

            using SKData imageData = surface.Snapshot().Encode(SKEncodedImageFormat.Png, Configuration.ImageQuality);
            imageData.SaveTo(File.OpenWrite("output.png"));
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
