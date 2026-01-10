using System;
using System.Collections.Generic;
using System.Text;
using AdofaiTheater.Foundation.Timeline;

namespace AdofaiTheater.Foundation.Basic
{
    public interface ITheaterHumanoidCharacter
    {
        public ITheaterEvent Walk(int frames, double deltaPositionX, double deltaPositionY);
        public ITheaterEvent Translate(int frames, double deltaPositionX, double deltaPositionY);
        public ITheaterEvent LayDown(int frames);
        public ITheaterEvent TurnAround(int frames);  // NOTE(seanlb): This might end up being an instant animation because linear algebra is tough.
    }
}
