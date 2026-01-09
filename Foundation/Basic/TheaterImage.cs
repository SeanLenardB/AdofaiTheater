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
        public string ImagePath { get; set; } = "";

        public override void Draw(SKCanvas canvas)
        {
            using (SKImage image = SKImage.FromEncodedData(this.ImagePath))
            {
                canvas.Save();
                canvas.Concat(this.Transform.TotalMatrix());
                canvas.DrawImage(image, SKPoint.Empty);
                canvas.Restore();
            }
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
