using System;
using System.Collections.Generic;
using System.Text;
using AdofaiTheater.Foundation.Basic;

namespace AdofaiTheater.Foundation.Timeline
{
	// TODO(seanlb): implement easing animations.
	/// <summary>
	/// A parameterized animation event. The parameter is in range (0,1].
	/// </summary>
	public class TheaterElementParameterizedAnimation : ITheaterAdjustableDurationEvent
	{
		public TheaterElementParameterizedAnimation(int totalFrames, Action<double> parameterizedAction)
		{
			this.TotalFrames = totalFrames;
			this.Action = parameterizedAction;
		}
		public TheaterElementParameterizedAnimation(Action<double> parameterizedAction)
		{
			this.Action = parameterizedAction;
		}

		public int Frame { get; private set; } = 0;
		public int TotalFrames { get; private set; } = 1;

		public Action<double> Action { get; set; } = (_) => { };

		public bool NextFrame()
		{
			this.Frame++;
			double parameter = (double)this.Frame / this.TotalFrames;
			if (this.Frame >= this.TotalFrames) { parameter = 1.0; }  // NOTE(seanlb): This is to prevent precision loss

			this.Action(parameter);

			return this.Frame < this.TotalFrames;
		}

		public void SetTotalFrames(int frames) { this.TotalFrames = frames; }
	}
}
