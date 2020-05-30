using System;
using UnityEngine;

namespace CCB.Roguelike
{
	[Serializable]
	public class CharacterSpriteComponent
	{
		[SerializeField]
		private string name = string.Empty;
		public string Name
		{
			get => name;
			private set => name = value;
		}

		public Texture2D SpriteSheet { get; private set; }

		public CharacterSpriteComponentType Type { get; private set; }

		public CharacterSpriteComponent(CharacterSpriteComponentType type, string name, Texture2D spriteSheet)
		{
			Type = type;
			Name = name;
			SpriteSheet = spriteSheet;
		}
	}
}