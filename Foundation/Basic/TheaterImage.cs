using AdofaiTheater.Foundation.Core;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace AdofaiTheater.Foundation.Basic
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
                this._ImageCache = null;
                field = value;
            }
        } = "";

        // NOTE(seanlb): This is a very important optimization.
        // Remember to mark the cache dirty if the image is changed.
        // Also remember to release the memory to prevent leaks.
        private SKImage? _ImageCache = null;
        public override void Draw(SKCanvas canvas)
        {
            this._ImageCache ??= SKImage.FromEncodedData(this.ImagePath);

            canvas.Save();
            canvas.Concat(this.Transform.TotalMatrix());
            canvas.DrawImage(this._ImageCache, SKPoint.Empty);
            canvas.Restore();
        }

        /// <summary>
        /// Rescales the image and puts it as the background.
        /// </summary>
        public TheaterImage AsBackground(Theater theater, BackgroundScalingPolicy scalingPolicy)
        {
            this.Transform.Layer = -1000;
            using (SKImage image = SKImage.FromEncodedData(this.ImagePath))
            {
                double widthScaleMultiplier = (double)theater.Configuration.Width / image.Width;
                double heightScaleMultiplier = (double)theater.Configuration.Height / image.Height;
                double finalScaleMultiplier = scalingPolicy switch
                {
                    BackgroundScalingPolicy.FILL_SCREEN => Math.Max(widthScaleMultiplier, heightScaleMultiplier),
                    BackgroundScalingPolicy.FIT_ONE_AXIS => Math.Min(widthScaleMultiplier, heightScaleMultiplier),
                    _ => throw new Exception("Unknown scaling policy!")
                };
				this.Transform.Matrix = SKMatrix.CreateScale((float)finalScaleMultiplier, (float)finalScaleMultiplier);
			}
            return this;
        }
        public enum BackgroundScalingPolicy { FILL_SCREEN, FIT_ONE_AXIS }
    }
}
