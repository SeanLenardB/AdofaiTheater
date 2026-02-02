using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using AdofaiTheater.Foundation.Core;
using AdofaiTheater.Foundation.Prefabs;
using AdofaiTheater.Foundation.Timeline;

namespace AdofaiTheater.Compiler
{
    public partial class TheaterCompiler
    {
        public Queue<string> CachedSubtitleLines { get; set; } = [];

        /// <summary>
        /// Reads all the lines in the given file as the subtitles and cache them. Then, in the script,
        /// call the method <see cref="TakeOneLineFromCache"/> to take one line from the cache.
        /// <br/><br/>
        /// This is equivalent of using <see cref="AppendSpeechAndSubtitle(string)"/> multiple times throughout the script.
        /// However, using a separate file for subtitles removes the hassle of switching between different input methods. That is very annoying.
        /// <br/><br/>
        /// <b>Empty lines will be ignored.</b> It's not recommended to have whitespace or empty lines in the subtitle file, because it may mess up your counting.
        /// But it can be helpful with making subtitle scripts more readable. You can check the return value for <see cref="TakeOneLineFromCache"/> to debug.
        /// </summary>
        public void CacheSubtitlesInFile(string file)
        {
            Debug.Assert(File.Exists(file), "This is not a valid file!");

            string[] lines = File.ReadAllLines(file);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) { continue; }
                this.CachedSubtitleLines.Enqueue(line);
            }
        }

        /// <summary>
        /// Use <see cref="CacheSubtitlesInFile(string)"/> before calling this method.
        /// </summary>
        /// <returns>The taken subtitle line from the cache for debugging timeline misalignments.</returns>
        public string TakeOneLineFromCache()
        {
            Debug.Assert(this.CachedSubtitleLines.Count > 0, "The subtitle cache is empty! Did you take in the file, or did you take too many lines?");
            string line = this.CachedSubtitleLines.Dequeue();
            this.AppendSpeechAndSubtitle(line);
            return line;
        }

        public TheaterCompiler AppendSpeechAndSubtitle(string speech)
        {
            this.AppendSpeech(speech);

            TheaterText speechElement = new TheaterText(speech).AsTheaterSubtitle(this.Theater);
            // TODO(seanlb): finish this after optimization
            this.AddElement($"_THEATER_SPEECH_INDEX_{this.Segments.Count - 1}_", speechElement);

            speechElement.Transform.Visible = false;

            // NOTE(seanlb): this is probably lacking in performance if there is too much of it.
            // We may need to separate OnSegmentAdvance, OnSegmentStart and OnSegmentEnd.
            //
            // Do not simplify this line and put it inside the lambda expression. Segments increase as you take more lines!
            ITheaterElement? previousElement = this.Elements.GetValueOrDefault($"_THEATER_SPEECH_INDEX_{this.Segments.Count - 2}_");
            this.AttachEventAutoDuration(T => new TheaterElementParameterizedAnimation(T, t =>
                {
                    if (t == 0d)
                    {
                        previousElement?.Transform.Visible = false;
                        speechElement.Transform.Visible = true;
                    }
                }));

            return this;
        }


    }
}
