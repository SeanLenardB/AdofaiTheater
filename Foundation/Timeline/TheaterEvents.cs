using System;
using System.Collections.Generic;
using System.Text;
using AdofaiTheater.Foundation.Basic;

namespace AdofaiTheater.Foundation.Timeline
{
	/// <summary>
	/// A parameterized animation event. The parameter is in range (0,1].
	/// If you specified <see cref="TheaterElementParameterizedAnimation.WithEase"/>,
	/// then the parameter will first be eased, then given to the action.
	/// </summary>
	public class TheaterElementParameterizedAnimation : ITheaterAdjustableDurationEvent
	{
		public TheaterElementParameterizedAnimation(Action<double> parameterizedAction)
		{
			this.Action = parameterizedAction;
		}
		public TheaterElementParameterizedAnimation(int totalFrames, Action<double> parameterizedAction)
		{
			this.TotalFrames = totalFrames;
			this.Action = parameterizedAction;
		}

		public int Frame { get; private set; } = 0;
		public int TotalFrames { get; private set; } = 1;

		public Action<double> Action { get; set; }

		public bool NextFrame()
		{
			this.Frame++;
			double parameter = (double)this.Frame / this.TotalFrames;
			double easedParameter = this.Eases.Aggregate(parameter, (t, ease) => ease.Ease(t));

			if (this.Frame >= this.TotalFrames) { easedParameter = 1.0; }  // NOTE(seanlb): This is to prevent precision loss
			this.Action(easedParameter);

			return this.Frame < this.TotalFrames;
		}



        public void SetTotalFrames(int frames) { this.TotalFrames = frames; }

		public List<IParameterizedEase> Eases { get; set; } = [];

		/// <summary>
		/// <b>Appends</b> the <paramref name="ease"/> to the end of the easing queue.
		/// </summary>
		public TheaterElementParameterizedAnimation WithEase(IParameterizedEase ease)
		{
            this.Eases.Add(ease);
            return this;
        }
    }
}
