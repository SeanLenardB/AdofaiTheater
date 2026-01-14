using System.Diagnostics;
using AdofaiTheater.Foundation.Basic;
using AdofaiTheater.Foundation.Timeline;
using FFMpegCore.Pipes;
using SkiaSharp;

namespace AdofaiTheater.Foundation.Core
{
    // NOTE(seanlb): This part only contains the necessary part of the animation of the theater.
    // Movement, Speaking and other events are done in Theater.Timeline.cs
    /// <summary>
    /// If you are very experienced with the framework <b>and</b> if you want more flexibility with the animation,
    /// then you should consider directly using this class.
    /// <br/><br/>
    /// For easier management, use <see cref="AdofaiTheater.Compiler.TheaterCompiler"/> instead.
    /// </summary>
    public partial class Theater
    {
        public Theater() { }



        public TheaterConfiguration Configuration { get; set; } = new();
        public Transform RootTransform { get; } = new();



        private readonly List<ITheaterElement> Elements = [];
        /// <summary>
        /// Animates and renders the scene.
        /// <br/><br/>
        /// See <see cref="https://swharden.com/csdv/skiasharp/video/"/> for more information
        /// </summary>
        /// <param name="onFrameEnd">returns <c>true</c> if the theater isn't done. <c>false</c> otherwise. The parameter <c>int</c> is the frame index (first frame is 1).</param>
        public IEnumerable<IVideoFrame> Animate(Predicate<int> onFrameEnd)
        {
            using (SKBitmap bitmap = new(this.Configuration.Width, this.Configuration.Height))
            {
                using (SKCanvas canvas = new(bitmap))
                {
                    int frameNumber = 0;
                    do  // NOTE(seanlb): a do-while loop is necessary to ensure that one-frame theaters can be rendered.
                    {
                        canvas.Clear();
                        frameNumber++;
                        foreach (
                            var element in
                            from element in this.Elements 
                            where element.Transform.Visible 
                            orderby element.Transform.Layer descending
                            select element
                            )
                        {
                            element.Draw(canvas);
                        }

                        yield return new SKBitmapFrame(bitmap);
                    }
                    while (onFrameEnd(frameNumber));
                }
            }
        }

        public void AddElement(ITheaterElement element)
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

        public int FramesPerSecond { get; set; } = 30;

        /// <summary>
        /// Ends <strong>WITHOUT</strong> slash.
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
            return Path.Combine(Directory.GetCurrentDirectory(), this.OutputPath, fileName);
        }

    }
}
