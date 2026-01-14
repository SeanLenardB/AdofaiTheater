using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace AdofaiTheater.Foundation.Core
{
    public class TheaterImage : TheaterElement
    {
        ~TheaterImage()
        {
            this._ImageCache?.Dispose();
        }

        public string ImagePath
        {
            get; set
            {
                this._ImageCache?.Dispose();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    this._ImageCache = SKImage.FromEncodedData(value);
                }
                field = value;
            }
        } = "";

		// NOTE(seanlb): This is a very important optimization.
		// Remember to mark the cache dirty if the image is changed.
		// Also remember to release the memory to prevent leaks.
		private SKImage? _ImageCache = null;
        public override void Draw(SKCanvas canvas)
        {
            canvas.Save();
            canvas.Concat(this.Transform.Matrix());
            canvas.DrawImage(this._ImageCache, SKPoint.Empty);
            canvas.Restore();
        }

        /// <summary>
        /// Rescales the image and puts it as the background.
        /// </summary>
        public TheaterImage AsBackground(Theater theater, BackgroundScalingPolicy scalingPolicy)
        {
            this.Transform.Layer = 1000;
            Debug.Assert(this._ImageCache is not null, "The cache is not null. You should assign an image first.");
            double widthScaleMultiplier = (double)theater.Configuration.Width / this._ImageCache.Width;
            double heightScaleMultiplier = (double)theater.Configuration.Height / this._ImageCache.Height;
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
            Debug.Assert(this._ImageCache is not null, "The cache is not null. You should assign an image first.");
            this.Transform.PivotAdd(this._ImageCache.Width / 2, this._ImageCache.Height / 2);
            return this;
        }

        public enum BackgroundScalingPolicy { FILL_SCREEN, FIT_ONE_AXIS }
    }
}
