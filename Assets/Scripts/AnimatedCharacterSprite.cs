using UnityEngine;

namespace CCB.Roguelike
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class AnimatedCharacterSprite : MonoBehaviour
	{
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

		[SerializeField]
		private int ticksPerFrame = 5;

		private SpriteRenderer spriteRenderer = null;
		private AnimationSpriteSet animations = null;
		private int currentFrame = 0;
		private int tickCounter = 0;
		private new Sprite[] animation = null;

		private void Awake()
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}

		private void FixedUpdate()
		{
			if (animations != null)
			{
				tickCounter = ++tickCounter % ticksPerFrame;

				if (tickCounter == 0)
				{
					currentFrame = ++currentFrame % animation.Length;
					spriteRenderer.sprite = animation[currentFrame];
				}
			}
		}

		public void OnRegenerateSprite()
		{
			spriteBuilder.Build(bodyType, bodyName, noseName, eyeName, hairName, earName, skinColour, eyeColour, hairColour,
			animationSpriteSet =>
			{
				animations = animationSpriteSet;
				currentFrame = 0;
				tickCounter = 0;
			});
			animation = animations[AnimationType.WalkDown];
		}
	}
}