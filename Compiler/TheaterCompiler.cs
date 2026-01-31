using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Versioning;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using AdofaiTheater.Foundation.Basic;
using AdofaiTheater.Foundation.Core;
using AdofaiTheater.Foundation.Prefabs;
using AdofaiTheater.Foundation.Timeline;
using FFMpegCore;
using FFMpegCore.Pipes;
using NAudio.Wave;

namespace AdofaiTheater.Compiler
{
    // TODO(seanlb): implement the script compiler
    [SupportedOSPlatform("windows")]
    public partial class TheaterCompiler
    {
        public Theater Theater { get; private set; } = new();

        private Dictionary<string, ITheaterElement> Elements { get; set; } = [];

        public TheaterCompiler AddElement<T>(string id, T element) where T : ITheaterElement
        {
            // NOTE(seanlb): This can become a compiler error.
            Debug.Assert(!this.Elements.ContainsKey(id), "Another element with the same id exists!");

            this.Elements.Add(id, element);

            this.Theater.AddElement(element);
            return this;
        }

        // WARN(seanlb): This is probably unnecessary because in the script you need to instantiate an element
        // in order to add it to the theater. There is no need for this function. Please reduce its calls.
        // If in the future, there is such need, this warn will get removed.
        public T GetElement<T>(string id) where T : class, ITheaterElement
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
        // 4. this.Compile();
        private List<TheaterSpeechSegment> Segments { get; set; } = [];

        public TheaterCompiler AppendSpeech(string speech)
        {
            TheaterSpeechSegment segment = 
                TheaterSpeechSynthesizer.Synthesize(speech, 
                    this.Theater.Configuration.ConcatenatePath($"Output_Audio_Segment_{this.Segments.Count}.wav"));
            this.Segments.Add(segment);

            return this;
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
            this.AttachEventAutoDuration(T => new TheaterElementParameterizedAnimation(T, t =>
                {
                    if (t == 0d)
                    {
                        this.Elements.GetValueOrDefault($"_THEATER_SPEECH_INDEX_{this.Segments.Count - 2}_")?.Transform.Visible = false;
                        speechElement.Transform.Visible = true;
                    }
                }));

            return this;
        }



        public TheaterCompiler AttachEvent(ITheaterEvent theaterEvent)
        {
            Debug.Assert(this.Segments.Count > 0, "You need to append a speech before attaching events!");
            this.Segments.Last().BoundEvents.Add(theaterEvent);
            return this;
        }

        /// <summary>
        /// <see cref="AttachEvent(ITheaterEvent)"/> but with more flexibility.
        /// The <paramref name="onTotalFramesDetermined"/>'s <see cref="int"/> parameter is the total frames once the audio is generated.
        /// Return the final <see cref="ITheaterEvent"/>.
        /// </summary>
        public TheaterCompiler AttachEventAutoDuration(Func<int, ITheaterEvent> onTotalFramesDetermined)
        {
            Debug.Assert(this.Segments.Count > 0, "You need to append a speech before attaching events!");
            this.AttachEvent(onTotalFramesDetermined((int)(this.Segments.Last().SpeechDuration.TotalSeconds * this.Theater.Configuration.FramesPerSecond)));
            return this;
        }


        public Queue<string> CachedSubtitleLines { get; set; } = [];

        /// <summary>
        /// Reads all the lines in the given file as the subtitles and cache them. Then, in the script,
        /// call the method <see cref="TakeOneLineFromCache"/> to take one line from the cache.
        /// <br/><br/>
        /// This is equivalent of using <see cref="AppendSpeechAndSubtitle(string)"/> multiple times throughout the script.
        /// However, using a separate file for subtitles removes the hassle of switching between different input methods. That is very annoying.
        /// <br/><br/>
        /// <b>Empty lines will be ignored.</b> It's not recommended to have whitespace or empty lines in the subtitle file, because it may mess up your counting.
        /// To prevent this error, empty lines will raise an assertion failure.
        /// </summary>
        public void CacheSubtitlesInFile(string file)
        {
            Debug.Assert(File.Exists(file), "This is not a valid file!");

            string[] lines = File.ReadAllLines(file);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    Debug.Assert(true, "There is one line in the subtitle file comprised of only whitespace or is completely empty! This will probably lead to unexpected timelines!");
                    continue;
                }
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
    }

    public class TheaterSpeechSegment
    {
        public TimeSpan SpeechDuration { get; set; } = TimeSpan.Zero;
        public string SpeechFileLocation { get; set; } = "";

        public List<ITheaterEvent> BoundEvents { get; set; } = [];
    }
}
