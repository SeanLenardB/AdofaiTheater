using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Speech.Synthesis;
using System.Text;
using AdofaiTheater.Foundation.Basic;
using AdofaiTheater.Foundation.Core;
using AdofaiTheater.Foundation.Timeline;

namespace AdofaiTheater.Compiler
{
    // TODO(seanlb): implement the script compiler
    public class TheaterCompiler
	{
		public Theater Theater { get; private set; } = new();

		private Dictionary<string, TheaterElement> Elements { get; set; } = [];

		public TheaterCompiler AddElement<T>(string id, T element) where T : TheaterElement
		{
			// NOTE(seanlb): This can become a compiler error.
			Debug.Assert(!this.Elements.ContainsKey(id), "Another element with the same id exists!");

			this.Elements.Add(id, element);

			this.Theater.AddElement(element);
			return this;
		}

		public T GetElement<T>(string id) where T : TheaterElement
		{
			// NOTE(seanlb): This can become a compiler error.
			// Also, I don't want a nullable type here.
			Debug.Assert(!this.Elements.ContainsKey(id), "The id does not exist!");
			Debug.Assert(this.Elements[id] is T, "The target element is not an instance of the generic type!");

			return (this.Elements[id] as T)!;
		}



		// NOTE(seanlb): A summary of the model:
		// 1. Speeches dominate the flow of the theater. That is, the timeline is driven by the speech.
		// 2. Each speech may be accompanied by some animations.
		//    (The animations are aligned to the start of the speech. To attach an event at the middle of the speech,
		//     split the speech into two segments and attach the event to the second one.)
		// 3. Empty speech is used for pauses.
		//
		// Steps to call the methods:
		// 1. this.AppendSpeech(...);
		// 2. this.AttachEvent(...);
		// 3. repeat the steps until the theater is done.
		// 4. this.Theater.Animate();  // NOTE(seanlb): This might be changed to this.Compile();
		private List<TheaterSpeechSegment> Segments { get; set; } = [];
		[SupportedOSPlatform("windows")]
		public TheaterCompiler AppendSpeech(string speech)
		{
			TheaterSpeechSegment segment = new()
			{
				SpeechFileLocation = this.Theater.Configuration.ConcatenatePath($"Output_Audio_Segment_{this.Segments.Count}.wav")
			};
			this.Segments.Add(segment);
			

			PromptBuilder builder = new();
			builder.AppendText(speech);
			builder.AppendBookmark("_SPEECH_END_");

			SpeechSynthesizer synthesizer = new();
			synthesizer.BookmarkReached += (o, e) =>
			{
				if (e.Bookmark != "_SPEECH_END_") { return; }  // NOTE(seanlb): For safety.
				segment.SpeechDuration = e.AudioPosition;
			};

			synthesizer.SetOutputToWaveFile(segment.SpeechFileLocation);
			synthesizer.Speak(builder);

			return this;
		}

		public TheaterCompiler AttachEvent(ITheaterEvent theaterEvent)
		{
			Debug.Assert(this.Segments.Count > 0, "You need to append a speech before attaching events!");
			this.Segments.Last().BoundEvents.Add(theaterEvent);
			return this;
		}

		public TheaterCompiler AttachEventAutoDuration(ITheaterAdjustableDurationEvent theaterEvent)
		{
			Debug.Assert(this.Segments.Count > 0, "You need to append a speech before attaching events!");
			theaterEvent.SetTotalFrames((int)(this.Segments.Last().SpeechDuration.TotalSeconds * this.Theater.Configuration.FramePerSecond));
			this.Segments.Last().BoundEvents.Add(theaterEvent);
			return this;
		}
	}

	public class TheaterSpeechSegment
	{
		public TimeSpan SpeechDuration { get; set; } = TimeSpan.Zero;
		public string SpeechFileLocation { get; set; } = "";

		public List<ITheaterEvent> BoundEvents { get; set; } = [];
	}
}
