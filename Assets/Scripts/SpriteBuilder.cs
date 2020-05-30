using System.Collections.Generic;
using UnityEngine;

namespace CCB.Roguelike
{
	[CreateAssetMenu(fileName = "Sprite Builder", menuName = "CCB/Singletons/Sprite Builder")]
	public class SpriteBuilder : ScriptableObject
	{
		[SerializeField]
		private Material spriteMaterial = null;

		private RenderTexture previousRenderTexture = null;
		private RenderTexture blitTarget = null;
		private int inUseBy = -1;

		public bool StartBuild(int user, int width, int height)
		{
			if (inUseBy == -1)
			{
				inUseBy = user;
				previousRenderTexture = RenderTexture.active;
				blitTarget = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
				RenderTexture.active = blitTarget;
				return true;
			}
			return false;
		}

		public void AddLayer(int user, Texture2D layer, Color tint)
		{
			if (inUseBy == user)
			{
				spriteMaterial.color = tint;
				Graphics.Blit(layer, blitTarget, spriteMaterial);
			}
		}

		public void PauseBuild(int user)
		{
			if (inUseBy == user)
			{
				RenderTexture.active = previousRenderTexture;
			}
		}

		public void FinishBuild(int user, Texture2D outputTexture)
		{
			if (inUseBy == user)
			{
				RenderTexture.active = previousRenderTexture;
				Graphics.CopyTexture(blitTarget, outputTexture);
				RenderTexture.ReleaseTemporary(blitTarget);
				blitTarget = null;
				previousRenderTexture = null;
				inUseBy = -1;
			}
		}

		public AnimationSpriteSet CreateAnimationSpriteSet(SpriteSheetDescription description, Texture2D spriteSheet)
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

			return new AnimationSpriteSet(animations);
		}

		private void OnEnable()
		{
			inUseBy = - 1;
		}
	}
}