using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CCB.Roguelike
{
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(Animation))]
	public class CharacterSprite : MonoBehaviour
	{
		// Don't store this here - use some kind of state object instead.
		[SerializeField]
		private Rigidbody2D characterRigidbody = null;

		[SerializeField]
		private UnityEvent onSpriteReady = null;

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

		private new Animation animation = null;
		private SpriteRenderer spriteRenderer = null;
		private AnimationType currentAnim = AnimationType.WalkDown;

		public void OnRegenerateSprite()
		{
			spriteBuilder.Build(bodyType, bodyName, noseName, eyeName, hairName, earName, skinColour, eyeColour, hairColour,
			animations =>
			{
				foreach (KeyValuePair<AnimationType, AnimationClip> animationPair in animations)
				{
					animation.AddClip(animationPair.Value, animationPair.Key.ToString());
				}
			});

			animation.wrapMode = WrapMode.Loop;
			animation.Play(AnimationType.WalkDown.ToString());
			StartCoroutine(Animate());
		}

		private void Awake()
		{
			animation = GetComponent<Animation>();
			spriteRenderer = GetComponent<SpriteRenderer>();
		}

		private void SetSprite(Sprite sprite)
		{
			spriteRenderer.sprite = sprite;
		}

		private IEnumerator Animate()
		{
			while (true)
			{
				Vector2 velocity = characterRigidbody.velocity;

				AnimationType anim = AnimationType.WalkDown;
				if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
				{
					if (Mathf.Abs(-velocity.x) > velocity.x)
						anim = AnimationType.WalkLeft;
					else
						anim = AnimationType.WalkRight;
				}
				else
				{
					if (velocity.y >= Mathf.Abs(-velocity.y))
						anim = AnimationType.WalkUp;
				}

				if (anim != currentAnim || !animation.isPlaying)
				{
					currentAnim = anim;
					animation.Play(currentAnim.ToString());
				}

				if (velocity.sqrMagnitude < 0.1f)
				{
					animation.Stop();
				}

				yield return new WaitForSeconds(0.1f);
			}
		}
	}
}