using System.Collections;
using UnityEngine;

namespace CCB.Roguelike
{
	public class CharacterAnimator : MonoBehaviour
	{
		[SerializeField]
		private Animator animator = null;

		[SerializeField]
		private Rigidbody2D characterRigidbody = null;

		[SerializeField]
		private PlayerCharacter playerCharacter = null;

		// Todo: This should be handled by some under-the-hood config file.
		[SerializeField]
		[Range(0.0f, 1.0f)]
		private float animationUpdateRate = 0.0f;

		private WaitForSeconds animUpdateWait = null;
		private int lookParamId = -1;
		private int speedParamId = -1;
		private int isMovingParamId = -1;
		private float lookX = 0.0f;
		private float lookY = -1.0f;
		private float velocityX = 0.0f;
		private float velocityY = 0.0f;
		private float speed = 0.0f;

		private void OnEnable()
		{
			lookParamId = Animator.StringToHash("MoveDir");
			speedParamId = Animator.StringToHash("Speed");
			isMovingParamId = Animator.StringToHash("IsMoving");

			StartCoroutine(Animate());
		}

		private IEnumerator Animate()
		{
			animUpdateWait = new WaitForSeconds(animationUpdateRate);

			while (enabled)
			{
				lookX = playerCharacter.LookDirection.x;
				lookY = playerCharacter.LookDirection.y;
				velocityX = Mathf.Clamp(characterRigidbody.velocity.x, -1, 1);
				velocityY = Mathf.Clamp(characterRigidbody.velocity.y, -1, 1);
				speed = characterRigidbody.velocity.sqrMagnitude;

				if (Mathf.Abs(lookX) > Mathf.Abs(lookY))
				{
					if (lookX > 0)
					{
						animator.SetInteger(lookParamId, (int)LookDir.Right);
						animator.SetBool(isMovingParamId, speed > 0.01f);
						animator.SetFloat(speedParamId, velocityX >= 0 ? speed : -speed);
					}
					else
					{
						animator.SetInteger(lookParamId, (int)LookDir.Left);
						animator.SetBool(isMovingParamId, speed > 0.01f);
						animator.SetFloat(speedParamId, velocityX <= 0 ? speed : -speed);
					}
				}
				else
				{
					if (lookY > 0)
					{
						animator.SetInteger(lookParamId, (int)LookDir.Up);
						animator.SetBool(isMovingParamId, speed > 0.01f);
						animator.SetFloat(speedParamId, velocityY >= 0 ? speed : -speed);
					}
					else
					{
						animator.SetInteger(lookParamId, (int)LookDir.Down);
						animator.SetBool(isMovingParamId, speed > 0.01f);
						animator.SetFloat(speedParamId, velocityY <= 0 ? speed : -speed);
					}
				}

				yield return animUpdateWait;
			}
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}

		private enum LookDir
		{
			Up, Down, Right, Left
		}
	}
}