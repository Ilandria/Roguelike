using System.Collections.Generic;
using UnityEngine;

namespace CCB.Roguelike
{
	[CreateAssetMenu(fileName = "New Character Sprite Builder", menuName = "CCB/Singleton/Character Sprite Builder")]
	public class CharacterSpriteBuilder : ScriptableObject
	{
		[SerializeField]
		private Material spriteMaterial = null;

		[SerializeField]
		private CharacterSpriteLayers spriteComponents = null;

		public (Texture2D, Dictionary<string, Sprite>) Build(CharacterBodyType bodyType, string bodyName, string noseName, string eyeName, string hairName, string earName, Color skinColour, Color eyeColour, Color hairColour)
		{
			int width = spriteComponents.SheetDescription.SpriteSheetSize.x;
			int height = spriteComponents.SheetDescription.SpriteSheetSize.y;
			RenderTexture previousRenderTexture = RenderTexture.active;
			Texture2D spriteSheet = new Texture2D(width, height, TextureFormat.RGBA32, false, false) { filterMode = FilterMode.Point };
			RenderTexture blitTarget = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
			RenderTexture.active = blitTarget;

			// Blit each layer from the repository to the temporary texture.
			spriteMaterial.color = skinColour;
			Graphics.Blit(spriteComponents.GetLayer(bodyType, CharacterPartType.Body, bodyName).SpriteSheet, blitTarget, spriteMaterial);
			Graphics.Blit(spriteComponents.GetLayer(bodyType, CharacterPartType.Nose, noseName).SpriteSheet, blitTarget, spriteMaterial);
			spriteMaterial.color = eyeColour;
			Graphics.Blit(spriteComponents.GetLayer(bodyType, CharacterPartType.Eyes, eyeName).SpriteSheet, blitTarget, spriteMaterial);
			spriteMaterial.color = hairColour;
			Graphics.Blit(spriteComponents.GetLayer(bodyType, CharacterPartType.Hair, hairName).SpriteSheet, blitTarget, spriteMaterial);
			spriteMaterial.color = skinColour;
			Graphics.Blit(spriteComponents.GetLayer(bodyType, CharacterPartType.Ears, earName).SpriteSheet, blitTarget, spriteMaterial);

			// Finalize the output texture.
			RenderTexture.active = previousRenderTexture;
			Graphics.CopyTexture(blitTarget, spriteSheet);
			RenderTexture.ReleaseTemporary(blitTarget);

			return CreateAnimationSpriteSet(spriteComponents.SheetDescription, spriteSheet);
		}

		private (Texture2D, Dictionary<string, Sprite>) CreateAnimationSpriteSet(SpriteSheetDescription description, Texture2D spriteSheet)
		{
			// Output for all generated animation-sprite sets.
			Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
			Rect frameBounds = Rect.zero;

			for (int animId = 0; animId < description.Animations.Count; animId++)
			{
				AnimationInfo animInfo = description.Animations[animId];

				for (int frameId = 0; frameId < animInfo.frameCount; frameId++)
				{
					frameBounds.Set(frameId * description.SpriteSize.x, animId * description.SpriteSize.y, description.SpriteSize.x, description.SpriteSize.y);
					Sprite sprite = Sprite.Create(spriteSheet, frameBounds, description.SpritePivot, 64, 0, SpriteMeshType.FullRect);
					sprite.name = $"{animInfo.type}{frameId}";
					sprites.Add(sprite.name, sprite);
				}
			}

			return (spriteSheet, sprites);
		}
	}
}