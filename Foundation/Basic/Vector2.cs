using System;
using System.Collections.Generic;
using System.Text;

namespace AdofaiTheater.Foundation.Basic
{
    public struct Vector2(double x, double y)
    {
        public double X { get; set; } = x;
        public double Y { get; set; } = y;

        public static Vector2 operator +(Vector2 left, Vector2 right) { return new(left.X + right.X, left.Y + right.Y); }
        public static Vector2 operator -(Vector2 left, Vector2 right) { return new(left.X - right.X, left.Y - right.Y); }
        public static Vector2 operator *(Vector2 vector, double multiplier) { return new(vector.X * multiplier, vector.Y * multiplier); }
        public static Vector2 operator /(Vector2 vector, double divisor) { return new(vector.X / divisor, vector.Y / divisor); }
    }
}
