using System.Diagnostics;
using AdofaiTheater.Foundation.Basic;
using AdofaiTheater.Foundation.Timeline;
using SkiaSharp;

namespace AdofaiTheater.Foundation.Core
{
    // NOTE(seanlb): This part only contains the necessary part of the animation of the theater.
    // Movement, Speaking and other events are done in Theater.Timeline.cs
    public partial class Theater
    {
        public Theater() { }



        public TheaterConfiguration Configuration { get; set; } = new();
        public Transform RootTransform { get; } = new();



        private readonly List<TheaterElement> Elements = [];
        public void Animate()
        {
            List<SKData> outputBuffer = [];

            using (SKSurface surface = SKSurface.Create(new SKImageInfo(this.Configuration.Width, this.Configuration.Height)))
            {
                int frameNumber = 0;
                do  // NOTE(seanlb): a do-while loop is necessary to ensure that one-frame theaters can be rendered.
                {
                    frameNumber++;
                    foreach (var element in this.Elements.OrderBy(e => e.Transform.Layer)) { element.Draw(surface.Canvas); }

                    outputBuffer.Add(surface.Snapshot().Encode(SKEncodedImageFormat.Png, this.Configuration.ImageQuality));
                    if (outputBuffer.Count >= this.Configuration.OutputBatchSize)
                    {
                        this.WriteOutputBuffer(outputBuffer, frameNumber);
                    }
                }
                while (this.NextFrame());

                if (outputBuffer.Count > 0)
                {
                    this.WriteOutputBuffer(outputBuffer, frameNumber);
                }
            }

        }
        private void WriteOutputBuffer(List<SKData> outputBuffer, int lastFrame)
        {
            Parallel.For(0, outputBuffer.Count, i =>
            {
                outputBuffer[i].SaveTo(File.OpenWrite(this.Configuration.ConcatenatePath($"Output_Frame_{lastFrame - outputBuffer.Count + i + 1}.png")));
            });
            Parallel.ForEach(outputBuffer, buffer => buffer.Dispose());
            outputBuffer.Clear();
        }

        public void AddElement(TheaterElement element)
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

        public int OutputBatchSize { get; set; } = 8;

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
