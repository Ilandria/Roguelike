using System;
using UnityEngine;

namespace CCB.Roguelike
{
	public class CharacterSpriteBuilderRequest
	{
		public static readonly int NumLayers = 5;
		public Action<AnimationSpriteSet> OnBuildComplete { get; set; } = null;
		public CharacterBodyType BodyType { get; set; } = CharacterBodyType.Error;
		public string BodyName { get; set; } = string.Empty;
		public string NoseName { get; set; } = string.Empty;
		public string EyeName { get; set; } = string.Empty;
		public string HairName { get; set; } = string.Empty;
		public string EarName { get; set; } = string.Empty;
		public Color SkinColour { get; set; } = Color.white;
		public Color EyeColour { get; set; } = Color.white;
		public Color HairColour { get; set; } = Color.white;
	}
}