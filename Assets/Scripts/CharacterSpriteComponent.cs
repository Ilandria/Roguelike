using System;
using UnityEngine;

namespace CCB.Roguelike
{
	[Serializable]
	public class CharacterSpriteComponent
	{
		[SerializeField]
		private CharacterSpriteComponentType type;
		public CharacterSpriteComponentType Type => type;

		[SerializeField]
		private Texture2D spriteSheet;
		public Texture2D SpriteSheet => spriteSheet;
	}
}