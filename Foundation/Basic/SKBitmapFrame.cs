using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using FFMpegCore.Pipes;
using SkiaSharp;

namespace AdofaiTheater.Foundation.Basic
{

    /// <summary>
    /// Refer to https://swharden.com/csdv/skiasharp/video/ and https://github.com/swharden/Csharp-Data-Visualization/blob/main/projects/skiasharp/video/GraphicsToVideo/SKBitmapFrame.cs
    /// </summary>
    public sealed class SKBitmapFrame : IVideoFrame, IDisposable
    {
        public int Width => this.Source.Width;
        public int Height => this.Source.Height;
        public string Format => "bgra";

        private readonly SKBitmap Source;

        public SKBitmapFrame(SKBitmap bmp)
        {
            Debug.Assert(bmp.ColorType == SKColorType.Bgra8888, "Only 'bgra' color type is supported!");
            this.Source = bmp;
        }

        public void Dispose() => this.Source.Dispose();
        public void Serialize(Stream stream) => stream.Write(this.Source.Bytes);
        public async Task SerializeAsync(Stream stream, CancellationToken token) =>
            await stream.WriteAsync(this.Source.Bytes, token).ConfigureAwait(false);

    }
}
