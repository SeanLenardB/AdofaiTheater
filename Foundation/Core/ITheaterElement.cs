using AdofaiTheater.Foundation.Basic;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AdofaiTheater.Foundation.Core
{
    public interface ITheaterElement
    {
        public Transform Transform { get; set; }

        public void Draw(SKCanvas canvas);
    }

    // NOTE(seanlb): This class might not end up being abstract.
    // I'm making them abstract because this forces me to implement some useful classes.

    public abstract class TheaterElementCollection : ITheaterElement
    {
        public Transform Transform { get; set; } = new();
        private List<ITheaterElement> Elements { get; set; } = [];

        public void Draw(SKCanvas canvas)
        {
            foreach (var element in this.Elements)
            {
                // NOTE(seanlb): This is temporarily disabled. Might support recursive elements for meme in the future.
                Debug.Assert(element != this, "Cannot have recursive elements!");

                element.Draw(canvas);
            }
        }

        public void Add(ITheaterElement element)
        {
            // NOTE(seanlb): I don't know whether this is necessary. Better strict than loose.
            Debug.Assert(element.Transform.Parent is null, "You are adding a non-dangling item to the collection! This might not be what you intended!");
            this.Elements.Add(element);
            element.Transform.Parent = this.Transform;
        }
    }
}
