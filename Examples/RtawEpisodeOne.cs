using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;
using AdofaiTheater.Compiler;
using AdofaiTheater.Foundation.Timeline;

namespace AdofaiTheater.Examples
{
    [SupportedOSPlatform("windows")]
    public static class RtawEpisodeOne
    {
        public static void EpisodeMain()
        {
            TheaterCompiler compiler = new();
            compiler.Theater.Configuration.OutputPath = "output";
            compiler.CacheSubtitlesInFile(@"Examples\script-Remake-1.txt");

            for (int i = 0; i < 4; i++)
            {
                compiler.TakeOneLineFromCache();
            }

            Console.WriteLine("Scripting done. Compilation begins.");

            compiler.Compile();
        }
    }
}
