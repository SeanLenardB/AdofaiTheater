using AdofaiTheater.Foundation.Basic;
using AdofaiTheater.Foundation.Core;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AdofaiTheater.Foundation.Drawing
{
    public class TheaterImage : TheaterElement
    {
        public string ImagePath { get; set; } = "";

        public override void Draw(SKCanvas canvas)
        {
            using (SKImage image = SKImage.FromEncodedData(this.ImagePath))
            {
                using (SKSurface bufferSurface = SKSurface.Create(new SKImageInfo(image.Width, image.Height)))
                {
                    bufferSurface.Canvas.DrawImage(image, SKPoint.Empty);
                    bufferSurface.Canvas.SetMatrix(this.Transform.Matrix);
                    canvas.DrawSurface(bufferSurface, SKPoint.Empty);
                }
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
