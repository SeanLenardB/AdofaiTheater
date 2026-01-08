using System;
using System.Collections.Generic;
using System.Text;

namespace AdofaiTheater.Foundation.Basic
{
    public class Transform
    {
        public Transform() { }
        public Transform(Transform parent) => Parent = parent;



        public Vector2 LocalTranslation { get; set; } = new();
        public Matrix2 LocalTransformation { get; set; } = new();

        public Transform? Parent { get; set; } = null;



        // WARN(seanlb): I have zero confidence in these two things. May not work as intended!
        public Vector2 GlobalTranslation()
        {
            if (Parent is null) { return LocalTranslation; }
            return Parent.GlobalTransformation() * Parent.GlobalTranslation() + LocalTranslation;
        }
        
        public Matrix2 GlobalTransformation()
        {
            if (Parent is null) { return LocalTransformation; }
            return Parent.GlobalTransformation() * LocalTransformation;
        }
    }
}
