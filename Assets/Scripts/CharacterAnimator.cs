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
		[Range(0.0f, 1.0f)]
		private float animationUpdateRate = 0.0f;

		private WaitForSeconds waitForSeconds = null;
		private MoveDir moveDir = MoveDir.Down;
		private float speed = 0.0f;
		private Vector2 velocity = Vector2.zero;
		private int moveParamId = -1;
		private int speedParamId = -1;

		private void OnEnable()
		{
			moveParamId = Animator.StringToHash("MoveDir");
			speedParamId = Animator.StringToHash("Speed");
			StartCoroutine(Animate());
		}

		private IEnumerator Animate()
		{
			waitForSeconds = new WaitForSeconds(animationUpdateRate);

			while (enabled)
			{
				velocity = characterRigidbody.velocity;

				// Speed.
				speed = velocity.sqrMagnitude;

				if (speed > 0.0001f)
				{
					// Movement direction.
					if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
					{
						if (velocity.x > 0)
						{
							moveDir = MoveDir.Right;
						}
						else
						{
							moveDir = MoveDir.Left;
						}
					}
					else
					{
						if (velocity.y > 0)
						{
							moveDir = MoveDir.Up;
						}
						else
						{
							moveDir = MoveDir.Down;
						}
					}
				}

				// Animator parameters.
				animator.SetInteger(moveParamId, (int)moveDir);
				animator.SetFloat(speedParamId, speed);

				yield return waitForSeconds;
			}
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}
		private enum MoveDir
		{
			Up, Down, Right, Left
		}
	}
}