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
        private Queue<TheaterSpeechSegment> Segments { get; set; } = [];
        [SupportedOSPlatform("windows")]
        public TheaterCompiler AppendSpeech(string speech)
        {
            TheaterSpeechSegment segment = new()
            {
                SpeechFileLocation = this.Theater.Configuration.ConcatenatePath($"Output_Audio_Segment_{this.Segments.Count + 1}.wav")
            };
            this.Segments.Enqueue(segment);

            PromptBuilder builder = new(new("zh-CN"));
            builder.AppendText(speech);
            builder.AppendBookmark("_SPEECH_END_");

            using (SpeechSynthesizer synthesizer = new())
            {
                synthesizer.SelectVoice("Microsoft Kangkang");
                synthesizer.Rate = 5;

                synthesizer.BookmarkReached += (o, e) =>
                {
                    if (e.Bookmark != "_SPEECH_END_") { return; }  // NOTE(seanlb): For safety.
                    segment.SpeechDurationFrames = (int)(e.AudioPosition.TotalSeconds * this.Theater.Configuration.FramesPerSecond);
                };

                synthesizer.SetOutputToWaveFile(segment.SpeechFileLocation);

                synthesizer.Speak(builder);

            }
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
            theaterEvent.SetTotalFrames(this.Segments.Last().SpeechDurationFrames);
            this.Segments.Last().BoundEvents.Add(theaterEvent);
            return this;
        }



        public void Compile()
        {
            TheaterSpeechSegment? currentSegment = null;
            this.Theater.Animate(t =>
            {
                if (currentSegment is not null && currentSegment.BoundEvents.Count == 0) { currentSegment = null; }
                if (this.Segments.Count == 0 && currentSegment is null) { return false; }

                currentSegment ??= this.Segments.Dequeue();

                // NOTE(seanlb): very good trick because the predicate will call the function once.
                currentSegment.BoundEvents.RemoveAll(e => !e.NextFrame());

                return true;
            });
        }
    }

    public class TheaterSpeechSegment
    {
        public int SpeechDurationFrames { get; set; } = 0;
        public string SpeechFileLocation { get; set; } = "";

        public List<ITheaterEvent> BoundEvents { get; set; } = [];
    }
}
