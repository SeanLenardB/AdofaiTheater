using System;
using System.Collections.Generic;
using System.Text;

namespace AdofaiTheater.src.Basic
{
    public struct Vector2(double x, double y)
    {
        public double X { get; set; } = x;
        public double Y { get; set; } = y;
    }
}
