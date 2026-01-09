using System;
using System.Collections.Generic;
using System.Text;
using AdofaiTheater.Foundation.Basic;

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

	// TODO(seanlb): implement easing animations.
	/// <summary>
	/// A parameterized animation event. The parameter is in range (0,1].
	/// </summary>
	public class TheaterElementParameterizedAnimation : ITheaterEvent
	{
		public TheaterElementParameterizedAnimation(int totalFrames, Action<double> parameterizedAction)
		{
			this.TotalFrames = totalFrames;
			this.Action = parameterizedAction;
		}

		public int Frame { get; private set; } = 0;
		public int TotalFrames { get; init; } = 1;

		public Action<double> Action { get; set; } = (_) => { };

		public bool NextFrame()
		{
			this.Frame++;
			double parameter = (double)this.Frame / this.TotalFrames;
			if (this.Frame >= this.TotalFrames) { parameter = 1.0; }  // NOTE(seanlb): This is to prevent precision loss

			this.Action(parameter);

			return this.Frame < this.TotalFrames;
		}
	}
}
