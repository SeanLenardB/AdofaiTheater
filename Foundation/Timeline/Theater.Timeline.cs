using System;
using System.Collections.Generic;
using System.Text;
using AdofaiTheater.Foundation.Timeline;

namespace AdofaiTheater.Foundation.Core
{
    // NOTE(seanlb): This part contains the core logic of the theater.
	// It is responsible for managing all the events that will happen throughout the entire video.
	// 
	// Currently, only linear timeline is supported. That is, only one event can happen at a time.
	// This is based upon the fact that the duration of the audio cannot be predetermined before running the script.
	// In the future, when the engine has a proper GUI or a custom scripting language (I'm not a great fan of),
	// we can probably look into improving this.
	//
	// When the theater is animated, it will repeatedly call the NextFrame() method, where the event queue
	// will be dequeued one after another. Every event is essentially an animation, or a function that will change
	// some states of some properties according to a given timestamp.
	public partial class Theater
	{
		private readonly Queue<ITheaterEvent> Events = [];

		private ITheaterEvent? CurrentEvent = null;
		/// <summary>
		/// Advances the theater by one frame.
		/// </summary>
		/// <returns><see cref="NextFrame">true</see> if the theater isn't done. <see cref="NextFrame">false</see> otherwise.</returns>
		public bool NextFrame()
		{
			if (this.Events.Count == 0 && this.CurrentEvent is null) { return false; }
			this.CurrentEvent ??= this.Events.Dequeue();

			if (!this.CurrentEvent.NextFrame()) { this.CurrentEvent = null; }

			return true;
		}

        public void PushEvent(ITheaterEvent theaterEvent)
        {
            this.Events.Enqueue(theaterEvent);
        }
	}
}
