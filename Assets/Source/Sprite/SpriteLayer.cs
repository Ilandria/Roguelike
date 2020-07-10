using System;
using UnityEngine;

namespace CCB.Roguelike
{
	[Serializable]
	public class SpriteLayer
	{
		public int Id { get; private set; }

		public string Name { get; private set; }

		public Texture2D SpriteSheet { get; private set; } = null;

		public BodyBase BodyBase { get; private set; }

		public BodyPart BodyPart { get; private set; }

		public SpriteLayer(BodyBase body, BodyPart part, int id, string name, Texture2D spriteSheet)
		{
			Id = id;
			BodyBase = body;
			BodyPart = part;
			Name = name;
			SpriteSheet = spriteSheet;
		}
	}
}