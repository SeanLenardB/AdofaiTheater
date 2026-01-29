using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;
using AdofaiTheater.Compiler;

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

            compiler.TakeOneLineFromCache();

            compiler.TakeOneLineFromCache();

            compiler.Compile();
        }
    }
}
