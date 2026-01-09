using System;
using System.Collections.Generic;
using System.Text;

namespace AdofaiTheater.Foundation.Timeline
{
	public interface ITheaterEvent
	{
		/// <summary>
		/// Advances the event by one frame.
		/// </summary>
		/// <returns><see cref="NextFrame">true</see> if the event isn't done. <see cref="NextFrame">false</see> otherwise.</returns>
		public bool NextFrame();
	}
}
