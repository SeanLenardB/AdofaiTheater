using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using AdofaiTheater.Foundation.Basic;
using AdofaiTheater.Foundation.Core;
using AdofaiTheater.Foundation.Timeline;
using SkiaSharp;

namespace AdofaiTheater.Foundation.Prefabs
{
	// NOTE(seanlb): this builder probably is redundant, but I will leave it here
	// because there is a possiblility where the characters need some auxiliary files.
	public class TheaterCharacterSimpleBuilder
	{
        public TheaterCharacterSimpleBuilder() { }

		private TheaterCharacterSimple? Character { get; set; } = null;

		/// <summary>
		/// See <see cref="/Resources/Patterns/CharacterSimple_Torso.png"/> for the <paramref name="torsoFile"/>.
		/// Pass that file directly into this method. The cutting and stitching will be automatically done.
		/// </summary>
		/// <param name="torsoFile"></param>
		/// <returns></returns>
        public TheaterCharacterSimpleBuilder WithResourceImage(string torsoFile)
		{
            Debug.Assert(this.Character is null, "You are going to overwrite another character! Did you call BuildCharacter(); ?");
            this.Character = new();

            using (SKImage torsoImage = SKImage.FromEncodedData(torsoFile))
            {
                Debug.Assert(torsoImage is not null, "No such file found!");

                this.Character.Body.UseSKImage(torsoImage.Subset(SKRectI.Create(1, 257, 254, 255)));
                this.Character.Body.Transform.PivotSet(142, 179);

				this.Character.LeftArm.UseSKImage(torsoImage.Subset(SKRectI.Create(257, 1, 133, 254)));
				this.Character.LeftArm.Transform.PivotSet(73, 65).PositionSet(49, -84).RotateClockwise(30);
				this.Character.LeftArm.Transform.Parent = this.Character.Body.Transform;

				this.Character.RightArm.UseSKImage(torsoImage.Subset(SKRectI.Create(392, 1, 120, 254)));
				this.Character.RightArm.Transform.PivotSet(52, 65).PositionSet(-39, -98).RotateClockwise(30);
				this.Character.RightArm.Transform.Parent = this.Character.Body.Transform;

                this.Character.Head.UseSKImage(torsoImage.Subset(SKRectI.Create(1, 1, 254, 254)));
				this.Character.Head.Transform.PivotSet(132, 196).PositionSet(13, -148);
				this.Character.Head.Transform.Parent = this.Character.Body.Transform;

				this.Character.LeftLeg.UseSKImage(torsoImage.Subset(SKRectI.Create(257, 257, 133, 255)));
				this.Character.LeftLeg.Transform.PivotSet(78, 77).PositionSet(35, 18);
				this.Character.LeftLeg.Transform.Parent = this.Character.Body.Transform;

				this.Character.RightLeg.UseSKImage(torsoImage.Subset(SKRectI.Create(392, 257, 120, 255)));
				this.Character.RightLeg.Transform.PivotSet(60, 79).PositionSet(-34, 9);
				this.Character.RightLeg.Transform.Parent = this.Character.Body.Transform;
            }

            return this;
        }

        public TheaterCharacterSimple BuildCharacter()
		{
			Debug.Assert(this.Character is not null, "You cannot build a character without specifying a resource image!");
			TheaterCharacterSimple character = this.Character;
			this.Character = null;
			return character;
		}
	}

    /// <summary>
    /// The general-purpose character that can appear in the theater.
    /// <br/><br/>
    /// <b>Do NOT instantiate this class directly! Use <see cref="TheaterCharacterSimpleBuilder"/>.</b>
    /// </summary>
    public class TheaterCharacterSimple : TheaterElementCollection, ITheaterHumanoidCharacter, ITheaterElement
	{
		// NOTE(seanlb): the default orientation of the character is towards -x, that is, left.
        public TheaterCharacterSimple()
        {
			this.Head.Transform.Layer = -2;
			this.LeftArm.Transform.Layer = -1;
			this.Body.Transform.Layer = 0;
			this.RightArm.Transform.Layer = 1;
			this.LeftLeg.Transform.Layer = 2;
			this.RightLeg.Transform.Layer = 3;

			this.Add(this.Head);
			this.Add(this.RightArm);
			this.Add(this.Body);
			this.Add(this.LeftArm);
			this.Add(this.RightLeg);
			this.Add(this.LeftLeg);
		}

		public TheaterImage Head { get; set; } = new();
		public TheaterImage Body { get; set; } = new();
		public TheaterImage LeftArm { get; set; } = new();
		public TheaterImage RightArm { get; set; } = new();
		public TheaterImage LeftLeg { get; set; } = new();
		public TheaterImage RightLeg { get; set; } = new();

        public ITheaterEvent LayDown(int frames)
		{
			throw new NotImplementedException();
		}

		public ITheaterEvent Translate(int frames, double deltaPositionX, double deltaPositionY)
		{
			throw new NotImplementedException();
		}

		public ITheaterEvent TurnAround(int frames)
		{
			throw new NotImplementedException();
		}

		public ITheaterEvent Walk(int frames, double deltaPositionX, double deltaPositionY)
		{
			throw new NotImplementedException();
		}
	}
}
