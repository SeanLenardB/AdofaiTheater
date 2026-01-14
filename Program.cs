using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using AdofaiTheater.Compiler;
using AdofaiTheater.Foundation.Basic;
using AdofaiTheater.Foundation.Core;
using AdofaiTheater.Foundation.Prefabs;
using AdofaiTheater.Foundation.Timeline;

namespace AdofaiTheater
{
    [SupportedOSPlatform("windows")]
    public class Program
    {
        private static void Main()
        {
            TheaterCompiler compiler = new();

            Stopwatch stopwatch = new();
            stopwatch.Start();    // element instantiation

            compiler.Theater.Configuration.OutputPath = "output";
            compiler.AddElement("bg",
                new TheaterImage()
                .UseFile(@"Resources/journeyend.png")
                .AsBackground(compiler.Theater, TheaterImage.BackgroundScalingPolicy.FILL_SCREEN));

            TheaterImage imageTrack = new TheaterImage().UseFile(@"Resources/adofaitrack.png");
            imageTrack.PivotAtCenter();
            compiler.AddElement("move", imageTrack);

            TheaterImage imageOneattempt = new TheaterImage().UseFile(@"Resources/oneattempt.png");
            imageOneattempt.PivotAtCenter();
            imageOneattempt.Transform.PositionSet(960, 540);
            imageOneattempt.Transform.Layer = 10;
            compiler.AddElement("meta", imageOneattempt);

            TheaterCharacterSimpleBuilder characterBuilder = new();
            TheaterCharacterSimple testCharacter =
                characterBuilder
                .WithResourceImage(@"Resources/Patterns/CharacterSimple_Torso.png")
                .BuildCharacter();
            testCharacter.Transform.PositionSet(960, 540);
            testCharacter.Transform.Layer = -11451;
            compiler.AddElement("character", testCharacter);

            compiler.AppendSpeechAndSubtitle("A simple text to test the text rendering.");
            compiler.AttachEventAutoDuration(T => new TheaterElementParameterizedAnimation(T, t =>
            {
                imageTrack.Transform.ScaleAdd(t / 50);
                testCharacter.LeftArm.Transform.RotateCounterClockwise(t * 2);
                compiler.Theater.RootTransform.RotateClockwise(0.3);
            }));

            compiler.AppendSpeechAndSubtitle("你的脸怎么红了？精神焕发！");
            compiler.AttachEventAutoDuration(T => new TheaterElementParameterizedAnimation(T, t =>
            {
                imageTrack.Transform.PositionSet(1500 * t, 300 * t);
                imageOneattempt.Transform.RotationSet(720 * t);
            }).WithEase(new InSineParameterizedEase()));
            compiler.AttachEventAutoDuration(T => testCharacter.Walk(T, -500, 0));

            compiler.AppendSpeechAndSubtitle("你的脸，怎么又黄了？我脸黄不黄跟你有关系吗");
            compiler.AttachEvent(new TheaterElementParameterizedAnimation(0, _ =>
            {
                imageOneattempt.Transform.Layer = -10;
            }));
            compiler.AttachEventAutoDuration(T => new TheaterElementParameterizedAnimation(T, t =>
            {
                imageTrack.Transform.ScaleMultiply(0.95, 0.95);
                imageOneattempt.Transform.RotationSet(-720 * t);
                testCharacter.Head.Transform.RotateClockwise(t * 5);
            }));
            compiler.AttachEventAutoDuration(T => new TheaterElementParameterizedAnimation(T, t =>
            {
                imageTrack.Transform.PositionSet(1500, 300 + (1000 * t));
            }).WithEase(new OutSineParameterizedEase()));
            compiler.AttachEventAutoDuration(T => testCharacter.Walk(T, 1200, 0));

            stopwatch.Stop();     // element instantiation
            Console.WriteLine($"Compilation:   {stopwatch.Elapsed.TotalSeconds}s.");

            stopwatch.Restart();  // theater rendering
            compiler.Compile();
            stopwatch.Stop();     // theater rendering
            Console.WriteLine($"Render:        {stopwatch.Elapsed.TotalSeconds}s.");
        }
    }
}