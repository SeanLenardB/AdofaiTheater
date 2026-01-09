using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdofaiTheater.Foundation.Drawing
{
    public class TheaterImage : TheaterElement
    {
        public string ImagePath { get; set; } = "";

        public override void Draw(SKCanvas canvas)
        {
            canvas.DrawImage(SKImage.FromEncodedData(this.ImagePath), SKPoint.Empty);
        }
    }
}
