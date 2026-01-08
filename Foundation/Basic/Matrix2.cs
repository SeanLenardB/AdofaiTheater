using System;
using System.Collections.Generic;
using System.Text;

namespace AdofaiTheater.Foundation.Basic
{
    public struct Matrix2
    {
        /// <summary>
        /// Initialize a matrix with given ihat and jhat
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public Matrix2(Vector2 i, Vector2 j)
        {
            I = i;
            J = j;
        }
        /// <summary>
        /// Initialize a matrix with the given rotation effect
        /// </summary>
        /// <param name="rotationRadians"></param>
        public Matrix2(double rotationRadians)
        {
            I = new(Math.Cos(rotationRadians), Math.Sin(rotationRadians));
            J = new(-Math.Sin(rotationRadians), Math.Cos(rotationRadians));
        }

        public Vector2 I { get; set; } = new(1, 0);
        public Vector2 J { get; set; } = new(0, 1);



        public void RotateCounterclockwise(double rotationRadians) { this = new Matrix2(rotationRadians) * this; }
        public void RotateClockwise(double rotationRadians) { this = new Matrix2(-rotationRadians) * this; }


        public static Matrix2 operator +(Matrix2 left, Matrix2 right) { return new(left.I + right.I, left.J + right.J); }
        public static Matrix2 operator -(Matrix2 left, Matrix2 right) { return new(left.I - right.I, left.J - right.J); }
        public static Vector2 operator *(Matrix2 matrix, Vector2 vector) { return matrix.I * vector.X + matrix.J * vector.Y; }
        public static Matrix2 operator *(Matrix2 left, Matrix2 right) { return new(left * right.I, left * right.J); }
        public static Matrix2 operator *(Matrix2 matrix, double multiplier) { return new(matrix.I * multiplier, matrix.J * multiplier); }
        public static Matrix2 operator /(Matrix2 matrix, double divisor) { return new(matrix.I / divisor, matrix.J / divisor); }

        public static bool operator ==(Matrix2 left, Matrix2 right) { return left.I == right.I && left.J == right.J; }
        public static bool operator !=(Matrix2 left, Matrix2 right) { return !(left == right); }

    }
}
