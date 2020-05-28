using System.Collections.Generic;
using UnityEngine;

namespace CCB.Roguelike
{
	public class AnimationSpriteSet
	{
		private static readonly Sprite[] emptySprite = new Sprite[] { };
		private IDictionary<AnimationType, Sprite[]> animations;

		public AnimationSpriteSet(IDictionary<AnimationType, Sprite[]> animations)
		{
			this.animations = animations;
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