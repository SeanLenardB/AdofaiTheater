using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Speech.Synthesis;
using System.Text;
using NAudio.Wave;

namespace AdofaiTheater.Compiler
{
    public class TheaterSpeechSynthesizer
    {
        [SupportedOSPlatform("windows")]
        public static TheaterSpeechSegment Synthesize(string speech, string outputFile)
        {
            TheaterSpeechSegment segment = new() { SpeechFileLocation = outputFile };

            string speechSsml = $@"
                <speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='zh-CN'>
                    <voice name='Microsoft Kangkang'>
                        <prosody pitch='-100%'>
                            {speech}
                        </prosody>
                    </voice>
                </speak>";

            using (SpeechSynthesizer synthesizer = new())
            {
                synthesizer.Volume = 100;
                synthesizer.Rate = 5;
                synthesizer.SetOutputToWaveFile(segment.SpeechFileLocation);
                synthesizer.SpeakSsml(speechSsml);
            }

            // NOTE(seanlb): the original bookmark is shit.
            // I have to rely on other people's power to do the .wav length acquisition.
            using (AudioFileReader reader = new(segment.SpeechFileLocation))
            {
                segment.SpeechDuration = reader.TotalTime;
            }

            return segment;
        }
    }
}
