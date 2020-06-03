using System.Collections.Generic;
using UnityEngine;

namespace CCB.Roguelike
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class CharacterSprite : MonoBehaviour
	{
		[SerializeField]
		private SpriteRenderer spriteRenderer = null;

		[SerializeField]
		private CharacterSpriteBuilder spriteBuilder = null;

		[SerializeField]
		private CharacterBodyType bodyType = CharacterBodyType.Error;

		[SerializeField]
		private string bodyName = string.Empty;

		[SerializeField]
		private string earName = string.Empty;

		[SerializeField]
		private string noseName = string.Empty;

		[SerializeField]
		private string eyeName = string.Empty;

		[SerializeField]
		private string hairName = string.Empty;

		[SerializeField]
		private Color skinColour = Color.white;

		[SerializeField]
		private Color eyeColour = Color.white;

		[SerializeField]
		private Color hairColour = Color.white;

		public Texture2D SpriteSheet { get; private set; }
		private Dictionary<string, Sprite> sprites = null;

		public void OnRegenerateSprite()
		{
			(Texture2D, Dictionary<string, Sprite>) result = spriteBuilder.Build(bodyType, bodyName, noseName, eyeName, hairName, earName, skinColour, eyeColour, hairColour);
			SpriteSheet = result.Item1;
			sprites = result.Item2;
		}

		private void LateUpdate()
		{
			if (sprites != null && sprites.TryGetValue(spriteRenderer.sprite.name, out Sprite sprite))
			{
				spriteRenderer.sprite = sprite;
			}
		}
	}
}