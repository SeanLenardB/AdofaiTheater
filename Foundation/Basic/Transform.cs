using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SkiaSharp;

namespace AdofaiTheater.Foundation.Basic
{
    public class Transform
    {
        public Transform() { }
        public Transform(Transform parent) => this.Parent = parent;



        /// <summary>
        /// Z axis of the element. Smaller number (negative number) means front.
        /// The default layer is 0.
        /// </summary>
        public int Layer { get; set; } = 0;
        public Vector2 Pivot { get; set; } = Vector2.Zero;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Vector2 Scale { get; set; } = Vector2.One;
        /// <summary>
        /// Counterclockwise direction is positive.
        /// </summary>
        public double Rotation { get; set; } = 0;
        public Transform? Parent { get; set; } = null;
        // NOTE(seanlb): I double whether this belongs to the Transform class,
        // or should be a separate event to toggle on/off the element.
        public bool Visible { get; set; } = true;



        // NOTE(seanlb):
        // In SkiaSharp, the drawing is done by specifying the topleft corner of the thing you want to draw,
        // and that is very annoying. So to counter that, we introduced the Pivot feature.
        // 
        // What we do to draw the thing is by first doing all those mathematical transformation, and then
        // move the image by its pivot and draw to the correct position.
        // However, that does not align with maths. So what we are doing here is splitting the transformation into
        // two different steps.
        //
        // LogicalMatrix is the same with mathematical formulas. Then the Matrix is where we really draw the things.
        public SKMatrix Matrix()
        {
            return SKMatrix.Concat(SKMatrix.CreateTranslation(-this.Pivot.X, -this.Pivot.Y), this.LogicalMatrix());
        }

        private SKMatrix LogicalMatrix()
        {
            SKMatrix currentMatrix = this.LocalLogicalMatrix();
            if (this.Parent is null) { return currentMatrix; }
            return SKMatrix.Concat(this.Parent.LogicalMatrix(), currentMatrix);
        }

        private SKMatrix LocalLogicalMatrix()
        {
            return SKMatrix.Concat(
                SKMatrix.CreateTranslation(this.Position.X, this.Position.Y),
                SKMatrix.Concat(
                    SKMatrix.CreateTranslation(this.Pivot.X, this.Pivot.Y),
                    SKMatrix.Concat(
                        SKMatrix.CreateRotationDegrees((float)this.Rotation),
                        SKMatrix.Concat(
                            SKMatrix.CreateScale(this.Scale.X, this.Scale.Y),
                            SKMatrix.CreateTranslation(-this.Pivot.X, -this.Pivot.Y)))));
        }

        public Transform PositionSet(Vector2 newPosition)
        {
            this.Position = newPosition;
            return this;
        }
        public Transform PositionSet(double x, double y) => this.PositionSet(new((float)x, (float)y));
        public Transform PositionAdd(Vector2 deltaPosition)
        {
            this.Position += deltaPosition;
            return this;
        }
        public Transform PositionAdd(double deltaX, double deltaY) => this.PositionAdd(new((float)deltaX, (float)deltaY));

        public Transform RotationSet(double angleDegrees)
        {
            this.Rotation = angleDegrees;
            return this;
        }
        public Transform RotateClockwise(double angleDegrees)
        {
            this.Rotation += angleDegrees;
            return this;
        }
        public Transform RotateCounterClockwise(double angleDegrees) => this.RotateClockwise(-angleDegrees);

        public Transform ScaleMultiply(Vector2 scaleMultiplier)
        {
            this.Scale = new(this.Scale.X * scaleMultiplier.X, this.Scale.Y * scaleMultiplier.Y);
            return this;
        }
        public Transform ScaleMultiply(double scaleMultiplierX, double scaleMultiplierY) => this.ScaleMultiply(new Vector2((float)scaleMultiplierX, (float)scaleMultiplierY));
        public Transform ScaleMultiply(double scaleMultiplier) => this.ScaleMultiply(new Vector2((float)scaleMultiplier, (float)scaleMultiplier));

        public Transform ScaleAdd(Vector2 scaleAdder)
        {
            this.Scale = new(this.Scale.X + scaleAdder.X, this.Scale.Y + scaleAdder.Y);
            return this;
        }
        public Transform ScaleAdd(double scaleAdderX, double scaleAdderY) => this.ScaleAdd(new Vector2((float)scaleAdderX, (float)scaleAdderY));
        public Transform ScaleAdd(double newScale) => this.ScaleAdd(new Vector2((float)newScale, (float)newScale));

        public Transform ScaleSet(Vector2 newScale)
        {
            this.Scale = newScale;
            return this;
        }
        public Transform ScaleSet(double newScaleX, double newScaleY) => this.ScaleSet(new Vector2((float)newScaleX, (float)newScaleY));
        public Transform ScaleSet(double newScale) => this.ScaleAdd(new Vector2((float)newScale, (float)newScale));

        public Transform PivotSet(Vector2 newPivot)
        {
            this.Pivot = newPivot;
            return this;
        }
        public Transform PivotSet(double newX, double newY) => this.PivotSet(new((float)newX, (float)newY));

        public Transform PivotAdd(Vector2 deltaPivot)
        {
            this.Pivot += deltaPivot;
            return this;
        }
        public Transform PivotAdd(double deltaX, double deltaY) => this.PivotAdd(new((float)deltaX, (float)deltaY));
    }
}
