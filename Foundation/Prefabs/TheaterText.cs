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
        public Transform Transform { get; set; } = new();

        public void Draw(SKCanvas canvas)
        {
            throw new NotImplementedException();
        }
    }
}
