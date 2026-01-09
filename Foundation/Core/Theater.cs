using System.Diagnostics;
using AdofaiTheater.Foundation.Basic;
using AdofaiTheater.Foundation.Drawing;
using SkiaSharp;

namespace AdofaiTheater.Foundation.Core
{
    public class Theater
    {
        public Theater() { }



        public TheaterConfiguration Configuration { get; set; } = new();
        public Transform RootTransform { get; } = new();



        public List<TheaterElement> Elements { get; private set; } = [];
        public void Animate()
        {
            using (SKSurface surface = SKSurface.Create(new SKImageInfo(this.Configuration.Width, this.Configuration.Height)))
            {
                foreach (var element in this.Elements.OrderByDescending(e => e.Transform.Layer)) { element.Draw(surface.Canvas); }

                using (SKData imageData = surface.Snapshot().Encode(SKEncodedImageFormat.Png, this.Configuration.ImageQuality))
                {
                    imageData.SaveTo(File.OpenWrite(this.Configuration.ConcatenatePath("output.png")));
                }
            }
        }

        public void Add(TheaterElement element)
        {
            // NOTE(seanlb): this class can inherit from TheaterElementCollection, but I don't feel like it.
            Debug.Assert(element.Transform.Parent is null, "You are adding a non-dangling item to the theater! This might not be what you intended!");
            this.Elements.Add(element);
            element.Transform.Parent = this.RootTransform;
        }
    }

    public class TheaterConfiguration
    {
        public int Width { get; set; } = 1920;
        public int Height { get; set; } = 1080;
        // NOTE(seanlb): I have no idea what this is
        public int ImageQuality { get; set; } = 100;

        /// <summary>
        /// Ends <strong>without</strong> slash.
        /// </summary>
        public string OutputPath
        {
            get; set
            {
                if (value.Length != 0)
                {
                    Debug.Assert(!(value.EndsWith('/') || value.EndsWith('\\')), "The output path should NOT end with a slash.");
                    if (!Directory.Exists(value)) { Directory.CreateDirectory(value); }
                }
                field = value;  // NOTE(seanlb): Sweet C# features always surprise me
            }
        } = "";
        public string ConcatenatePath(string fileName)
        {
            if (this.OutputPath.Length == 0) { return fileName; }
            return this.OutputPath + '/' + fileName;
        }

    }
}
