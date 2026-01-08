using AdofaiTheater.Foundation.Basic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AdofaiTheater.Foundation.Drawing
{
    // NOTE(seanlb): These two classes might not end up being abstract.
    // I'm making them abstract because this forces me to implement some useful classes.
    public abstract class TheaterElement
    {
        public Transform Transform { get; set; } = new();
        /// <summary>
        /// Z axis, if you like. Smaller number means front.
        /// </summary>
        public int Layer { get; set; } = 0;

        public virtual void Draw() { }
    }

    public abstract class TheaterElementCollection : TheaterElement
    {
        private List<TheaterElement> Elements { get; set; } = [];

        public override void Draw()
        {
            foreach (var element in Elements)
            {
                // NOTE(seanlb): This is temporarily disabled. Might support recursive elements for meme in the future.
                Debug.Assert(element != this, "Cannot have recursive elements!");

                element.Draw();
            }
        }

        public void Add(TheaterElement element)
        {
            // NOTE(seanlb): I don't know whether this is necessary. Better strict than loose.
            Debug.Assert(element.Transform.Parent is null, "You are adding a non-dangling item to the colleciton! This might not be what you intended!");
            element.Transform.Parent = Transform;
        }
    }
}
