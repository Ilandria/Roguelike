using System;
using System.Collections;
using UnityEngine;

namespace CCB.Roguelike
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class AnimatedCharacterSprite : MonoBehaviour
	{
		[SerializeField]
		private SpriteBuilder spriteBuilder = null;

		[SerializeField]
		private SpriteSheetDescription spriteSheetDescription = null;

		[SerializeField]
		private Texture2D bodySheet = null;

		[SerializeField]
		private Texture2D earSheet = null;

		[SerializeField]
		private Texture2D noseSheet = null;

		[SerializeField]
		private Texture2D eyeSheet = null;

		[SerializeField]
		private Texture2D hairSheet = null;

		[SerializeField]
		private Color skinColour = Color.white;

		[SerializeField]
		private Color eyeColour = Color.white;

		[SerializeField]
		private Color hairColour = Color.white;

		[SerializeField]
		private AnimationType currentAnimation = AnimationType.WalkDown;

		[SerializeField]
		private int ticksPerFrame = 5;

		private Texture2D spriteSheet;
		private SpriteRenderer spriteRenderer = null;
		private AnimationSpriteSet animations = null;
		private int currentFrame = 0;
		private int tickCounter = 0;

		private void Awake()
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
			spriteSheet = new Texture2D(bodySheet.width, bodySheet.height, TextureFormat.RGBA32, false, false) { filterMode = FilterMode.Point };
			StartCoroutine(BuildNewSprite());
		}

		private void FixedUpdate()
		{
			tickCounter = ++tickCounter % ticksPerFrame;

			if (tickCounter == 0)
			{
				currentFrame = ++currentFrame % animations[currentAnimation].Length;
				spriteRenderer.sprite = animations[currentAnimation][currentFrame];
			}
		}

		public void OnRegenerateSprite()
		{
			StartCoroutine(BuildNewSprite());
		}

		public void OnNextAnimation()
		{
			currentFrame = 0;
			tickCounter = 0;
			currentAnimation = (AnimationType)((int)++currentAnimation % Enum.GetNames(typeof(AnimationType)).Length);
		}

		public void OnPreviousAnimation()
		{
			currentFrame = 0;
			tickCounter = 0;
			currentAnimation = --currentAnimation >= 0 ? currentAnimation : (AnimationType)(Enum.GetNames(typeof(AnimationType)).Length - 1);
		}

		private IEnumerator BuildNewSprite()
		{
			int id = GetInstanceID();
			yield return new WaitUntil(() => spriteBuilder.StartBuild(id, spriteSheet.width, spriteSheet.height));
			spriteBuilder.AddLayer(id, bodySheet, skinColour);
			spriteBuilder.AddLayer(id, noseSheet, skinColour);
			spriteBuilder.AddLayer(id, eyeSheet, eyeColour);
			spriteBuilder.AddLayer(id, hairSheet, hairColour);
			spriteBuilder.AddLayer(id, earSheet, skinColour);
			spriteBuilder.PauseBuild(id);
			yield return new WaitForEndOfFrame();
			spriteBuilder.FinishBuild(id, spriteSheet);
			animations = spriteBuilder.CreateAnimationSpriteSet(spriteSheetDescription, spriteSheet);
			currentFrame = 0;
			tickCounter = 0;
		}
	}
}