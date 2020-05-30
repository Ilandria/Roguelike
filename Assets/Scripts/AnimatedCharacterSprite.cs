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
		private CharacterSpriteComponents sprites = null;

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
			spriteSheet = new Texture2D(sprites.SheetDescription.SpriteSheetSize.x, sprites.SheetDescription.SpriteSheetSize.y, TextureFormat.RGBA32, false, false) { filterMode = FilterMode.Point };
		}

		private void FixedUpdate()
		{
			if (animations != null)
			{
				tickCounter = ++tickCounter % ticksPerFrame;

				if (tickCounter == 0)
				{
					currentFrame = ++currentFrame % animations[currentAnimation].Length;
					spriteRenderer.sprite = animations[currentAnimation][currentFrame];
				}
			}
		}

		public void RegenerateSprite()
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
			// Todo: Find a better way to handle sex - ideally be able to send in a "male" or "female" flag instead of per-type. Maybe split parts and sex into 2 enums.
			spriteBuilder.AddLayer(id, sprites.GetSpriteComponent(CharacterSpriteComponentType.FemaleBody, bodyName).SpriteSheet, skinColour);
			spriteBuilder.AddLayer(id, sprites.GetSpriteComponent(CharacterSpriteComponentType.FemaleNose, noseName).SpriteSheet, skinColour);
			spriteBuilder.AddLayer(id, sprites.GetSpriteComponent(CharacterSpriteComponentType.FemaleEyes, eyeName).SpriteSheet, eyeColour);
			spriteBuilder.AddLayer(id, sprites.GetSpriteComponent(CharacterSpriteComponentType.FemaleHair, hairName).SpriteSheet, hairColour);
			spriteBuilder.AddLayer(id, sprites.GetSpriteComponent(CharacterSpriteComponentType.FemaleEars, earName).SpriteSheet, skinColour);
			spriteBuilder.PauseBuild(id);
			yield return new WaitForEndOfFrame();
			spriteBuilder.FinishBuild(id, spriteSheet);
			animations = spriteBuilder.CreateAnimationSpriteSet(sprites.SheetDescription, spriteSheet);
			currentFrame = 0;
			tickCounter = 0;
		}
	}
}