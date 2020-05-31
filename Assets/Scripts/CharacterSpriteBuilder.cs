using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCB.Roguelike
{
	public class CharacterSpriteBuilder : MonoBehaviour
	{
		[SerializeField]
		private Material spriteMaterial = null;

		[SerializeField]
		private CharacterSpriteComponents spriteComponents = null;

		[SerializeField]
		private Queue<CharacterSpriteBuilderRequest> currentRequests = new Queue<CharacterSpriteBuilderRequest>();

		private bool isBuilding = false;

		public void Request(CharacterBodyType bodyType, string bodyName, string noseName, string eyeName, string hairName, string earName, Color skinColour, Color eyeColour, Color hairColour, Action<AnimationSpriteSet> onRequestComplete)
		{
			currentRequests.Enqueue(new CharacterSpriteBuilderRequest
			{
				OnBuildComplete = onRequestComplete,
				BodyType = bodyType,
				BodyName = bodyName,
				NoseName = noseName,
				EyeName = eyeName,
				HairName = hairName,
				EarName = earName,
				SkinColour = skinColour,
				EyeColour = eyeColour,
				HairColour = hairColour
			});

			if (!isBuilding)
			{
				isBuilding = true;
				StartCoroutine(ProcessRequests());
			}
		}

		private AnimationSpriteSet CreateAnimationSpriteSet(SpriteSheetDescription description, Texture2D spriteSheet)
		{
			// Output for all generated animation-sprite sets.
			Dictionary<AnimationType, Sprite[]> animations = new Dictionary<AnimationType, Sprite[]>();
			Rect frameBounds = Rect.zero;

			for (int animId = 0; animId < description.Animations.Count; animId++)
			{
				AnimationInfo animInfo = description.Animations[animId];
				Sprite[] frames = new Sprite[animInfo.frameCount];

				for (int frameId = 0; frameId < animInfo.frameCount; frameId++)
				{
					frameBounds.Set(frameId * description.SpriteSize.x, animId * description.SpriteSize.y, description.SpriteSize.x, description.SpriteSize.y);
					frames[frameId] = Sprite.Create(spriteSheet, frameBounds, description.SpritePivot);
				}

				// Add each animation type and all of its frames (sprites) to the output.
				animations.Add(animInfo.type, frames);
			}

			return new AnimationSpriteSet(animations, spriteSheet);
		}

		private IEnumerator ProcessRequests()
		{
			while (currentRequests.Count > 0)
			{
				CharacterSpriteBuilderRequest request = currentRequests.Dequeue();
				int width = spriteComponents.SheetDescription.SpriteSheetSize.x;
				int height = spriteComponents.SheetDescription.SpriteSheetSize.y;
				RenderTexture previousRenderTexture = RenderTexture.active;
				Texture2D spriteSheet = new Texture2D(width, height, TextureFormat.RGBA32, false, false) { filterMode = FilterMode.Point };
				RenderTexture blitTarget = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
				RenderTexture.active = blitTarget;

				// Blit each layer from the repository to the temporary texture.
				spriteMaterial.color = request.SkinColour;
				Graphics.Blit(spriteComponents.GetSpriteSheet(request.BodyType, CharacterPartType.Body, request.BodyName).SpriteSheet, blitTarget, spriteMaterial);
				Graphics.Blit(spriteComponents.GetSpriteSheet(request.BodyType, CharacterPartType.Nose, request.NoseName).SpriteSheet, blitTarget, spriteMaterial);
				spriteMaterial.color = request.EyeColour;
				Graphics.Blit(spriteComponents.GetSpriteSheet(request.BodyType, CharacterPartType.Eyes, request.EyeName).SpriteSheet, blitTarget, spriteMaterial);
				spriteMaterial.color = request.HairColour;
				Graphics.Blit(spriteComponents.GetSpriteSheet(request.BodyType, CharacterPartType.Hair, request.HairName).SpriteSheet, blitTarget, spriteMaterial);
				spriteMaterial.color = request.SkinColour;
				Graphics.Blit(spriteComponents.GetSpriteSheet(request.BodyType, CharacterPartType.Ears, request.EarName).SpriteSheet, blitTarget, spriteMaterial);
				yield return null;

				// Finalize the output texture.
				RenderTexture.active = previousRenderTexture;
				Graphics.CopyTexture(blitTarget, spriteSheet);
				RenderTexture.ReleaseTemporary(blitTarget);
				blitTarget = null;
				previousRenderTexture = null;

				request?.OnBuildComplete(CreateAnimationSpriteSet(spriteComponents.SheetDescription, spriteSheet));
			}

			isBuilding = false;
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}
	}
}