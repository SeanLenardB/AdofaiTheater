using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Text;

namespace AdofaiTheater.Foundation.Speaking
{
	// TODO(seanlb): I haven't decided upon how this should work.
	public interface ITheaterSpeakable
	{
		/// <summary>
		/// Generates the speech and saves it into the file.
		/// </summary>
		public Task GenerateSpeech(string fileLocation, PromptBuilder prompt);
	}
}
