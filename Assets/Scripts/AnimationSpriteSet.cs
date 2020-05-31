using System.Collections.Generic;
using UnityEngine;

namespace CCB.Roguelike
{
	public class AnimationSpriteSet
	{
		private static readonly Sprite[] emptySprite = new Sprite[] { };
		private readonly IDictionary<AnimationType, Sprite[]> animations;
		public Texture2D SpriteSheet { get; private set; }

		public AnimationSpriteSet(IDictionary<AnimationType, Sprite[]> animations, Texture2D spriteSheet)
		{
			this.animations = animations;
			SpriteSheet = spriteSheet;
		}

		public Sprite[] this[AnimationType type]
		{
			get => animations[type] ?? emptySprite;
			set => Add(type, value);
		}

		public void Add(AnimationType type, Sprite[] frames) => animations.Add(type, frames);

		public void Has(AnimationType type) => animations.ContainsKey(type);
	}
}