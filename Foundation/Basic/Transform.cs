using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace AdofaiTheater.Foundation.Basic
{
    public class Transform
    {
        public Transform() { }
        public Transform(Transform parent) => this.Parent = parent;



        // NOTE(seanlb): It's not the best idea to invent the important wheel here.
        public SKMatrix Matrix { get; set; } = SKMatrix.Identity;

        /// <summary>
        /// Z axis of the element. Smaller number (negative number) means front.
        /// The default layer is 0.
        /// </summary>
        public int Layer { get; set; } = 0;

		public Transform? Parent { get; set; } = null;



        public SKMatrix TotalMatrix()
        {
            if (this.Parent is null) { return this.Matrix; }

            return SKMatrix.Concat(this.Parent.TotalMatrix(), this.Matrix);
        }
    }
}
