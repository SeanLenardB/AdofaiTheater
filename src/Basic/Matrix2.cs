using System;
using System.Collections.Generic;
using System.Text;

namespace AdofaiTheater.src.Basic
{
    public struct Matrix2
    {
        public Matrix2() { }

        public Vector2 I { get; set; } = new(1, 0);
        public Vector2 J { get; set; } = new(0, 1);
    }
}
