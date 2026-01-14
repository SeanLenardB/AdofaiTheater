using System;
using System.Collections.Generic;
using System.Text;
using AdofaiTheater.Foundation.Timeline;

namespace AdofaiTheater.Foundation.Core
{
    public interface ITheaterHumanoidCharacter
    {
        public ITheaterEvent Walk(int frames, double deltaPositionX, double deltaPositionY);
    }
}
