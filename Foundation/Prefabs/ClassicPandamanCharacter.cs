using System;
using System.Collections.Generic;
using System.Text;
using AdofaiTheater.Foundation.Core;
using AdofaiTheater.Foundation.Timeline;

namespace AdofaiTheater.Foundation.Prefabs
{
	public class ClassicPandamanCharacter : ITheaterHumanoidCharacter
	{
		public ITheaterEvent LayDown(int frames)
		{
			throw new NotImplementedException();
		}

		public ITheaterEvent Translate(int frames, double deltaPositionX, double deltaPositionY)
		{
			throw new NotImplementedException();
		}

		public ITheaterEvent TurnAround(int frames)
		{
			throw new NotImplementedException();
		}

		public ITheaterEvent Walk(int frames, double deltaPositionX, double deltaPositionY)
		{
			throw new NotImplementedException();
		}
	}
}
