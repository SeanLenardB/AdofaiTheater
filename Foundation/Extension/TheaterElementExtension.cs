using System;
using System.Collections.Generic;
using System.Text;
using AdofaiTheater.Foundation.Core;

namespace AdofaiTheater.Foundation.Extension
{
    public static class TheaterElementExtension
    {
        public static ITheaterElement Hide(this ITheaterElement element)
        {
            element.Transform.Visible = false;
            return element;
        }

        public static ITheaterElement Show(this ITheaterElement element)
        {
            element.Transform.Visible = true;
            return element;
        }
    }
}
