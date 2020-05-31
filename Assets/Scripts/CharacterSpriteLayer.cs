using System;
using UnityEngine;

namespace CCB.Roguelike
{
	[Serializable]
	public class CharacterSpriteLayer
	{
		[SerializeField]
		private string name = string.Empty;
		public string Name
		{
			get => name;
			private set => name = value;
		}

		public Texture2D SpriteSheet { get; private set; } = null;

		public CharacterBodyType Body { get; private set; }

		public CharacterPartType Part { get; private set; }

		public CharacterSpriteLayer(CharacterBodyType body, CharacterPartType part, string name, Texture2D spriteSheet)
		{
			Body = body;
			Part = part;
			Name = name;
			SpriteSheet = spriteSheet;
		}
	}
}