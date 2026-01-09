using System;
using System.Collections.Generic;
using System.Text;

namespace AdofaiTheater.Foundation.Timeline
{
	public class TheaterPauseEvent : ITheaterEvent
	{
		public TheaterPauseEvent(int pauseFrames)
		{
			this.RemainingFrames = pauseFrames;
		}

		public int RemainingFrames { get; private set; } = 0;

		public bool NextFrame()
		{
			this.RemainingFrames--;
			return this.RemainingFrames > 0;
		}
	}

	// TODO(seanlb): Implement character speech.
	public class TheaterCharacterSpeakEvent : ITheaterEvent
	{
		public bool NextFrame()
		{
			throw new NotImplementedException();
		}
	}

	// TODO(seanlb): Implement element movement.
	public class TheaterElementMoveEvent : ITheaterEvent
	{
		public bool NextFrame()
		{
			throw new NotImplementedException();
		}
	}
}
