using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using AdofaiTheater.Foundation.Basic;
using AdofaiTheater.Foundation.Core;

namespace AdofaiTheater.Compiler
{
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

		public TheaterCompiler AppendSpeech(string speech)
		{
			// TODO(seanlb): implement speech.

			return this;
		}

		// TODO(seanlb): implement the script compiler
		public TheaterCompiler AppendScript() { return this; }
	}
}
