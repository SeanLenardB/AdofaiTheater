using System;
using System.Collections.Generic;
using System.Text;
using AdofaiTheater.Foundation.Basic;
using AdofaiTheater.Foundation.Core;
using SkiaSharp;

namespace AdofaiTheater.Foundation.Prefabs
{
    public class TheaterText : ITheaterElement
    {
        public TheaterText(string text)
        {
            this.Text = text;
        }

        public string Text { get; set; } = "";
        public Transform Transform { get; set; } = new();
        public SKPaint Paint { get; set; } = new();

        public TheaterText AsTheaterSubtitle(Theater theater)
        {
            this.Transform.PositionSet(theater.Configuration.Width / 2, theater.Configuration.Height - 50);
            this.Transform.Layer = -114514;
            this.Paint.Color = SKColor.Parse("ffffff");

            return this;
        }

        public void Draw(SKCanvas canvas)
        {
            canvas.Save();
            canvas.Concat(this.Transform.Matrix());
            canvas.DrawText(
                this.Text,
                SKPoint.Empty,
                SKTextAlign.Center,
                new SKFont(SKTypeface.Default, size: 50),
                this.Paint
            );
            canvas.Restore();
        }
    }
}
