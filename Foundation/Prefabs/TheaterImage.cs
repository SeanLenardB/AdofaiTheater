using AdofaiTheater.Foundation.Basic;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace AdofaiTheater.Foundation.Core
{
    public class TheaterImage : ITheaterElement
    {
        ~TheaterImage()
        {
            this._Image?.Dispose();
        }

        public Transform Transform { get; set; } = new();

        // NOTE(seanlb): This is a very important optimization.
        // Remember to mark the cache dirty if the image is changed.
        // Also remember to release the memory to prevent leaks.
        private SKImage? _Image = null;
        public void Draw(SKCanvas canvas)
        {
            canvas.Save();
            canvas.Concat(this.Transform.Matrix());
            canvas.DrawImage(this._Image, SKPoint.Empty);
            canvas.Restore();
        }

        public TheaterImage UseFile(string image)
        {
            this._Image?.Dispose();
            if (!string.IsNullOrWhiteSpace(image))
            {
				Debug.Assert(image is not null, "No such file found!");
                this._Image = SKImage.FromEncodedData(image);
            }
            return this;
        }

        public TheaterImage UseSKImage(SKImage image)
        {
            this._Image?.Dispose();
            this._Image = image;
            return this;
        }

        /// <summary>
        /// Rescales the image and puts it as the background.
        /// </summary>
        public TheaterImage AsBackground(Theater theater, BackgroundScalingPolicy scalingPolicy)
        {
            this.Transform.Layer = 1000;
            Debug.Assert(this._Image is not null, "The cache is not null. You should assign an image first.");
            double widthScaleMultiplier = (double)theater.Configuration.Width / this._Image.Width;
            double heightScaleMultiplier = (double)theater.Configuration.Height / this._Image.Height;
            double finalScaleMultiplier = scalingPolicy switch
            {
                BackgroundScalingPolicy.FILL_SCREEN => Math.Max(widthScaleMultiplier, heightScaleMultiplier),
                BackgroundScalingPolicy.FIT_ONE_AXIS => Math.Min(widthScaleMultiplier, heightScaleMultiplier),
                _ => throw new Exception("Unknown scaling policy!")
            };
            this.Transform.ScaleSet(finalScaleMultiplier, finalScaleMultiplier);
            return this;
        }

        public TheaterImage PivotAtCenter()
        {
            Debug.Assert(this._Image is not null, "The cache is not null. You should assign an image first.");
            this.Transform.PivotAdd(this._Image.Width / 2, this._Image.Height / 2);
            return this;
        }

        public enum BackgroundScalingPolicy { FILL_SCREEN, FIT_ONE_AXIS }
    }
}
