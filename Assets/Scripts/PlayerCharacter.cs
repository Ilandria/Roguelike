using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace CCB.Roguelike
{
	public class PlayerCharacter : MonoBehaviourPun, IPunObservable
	{
		// Todo: This needs to be hooked up to character stats and not hard-coded.
		[SerializeField]
		[Range(0.0f, 0.25f)]
		private float turnSpeed = 0.5f;

		// Todo: This should be handled by some under-the-hood config file.
		[SerializeField]
		private float turnTargetUpdateRate = 0.25f;
		private WaitForSeconds turnUpdateWait = null;

		// Todo: Down the road I may want to make this not a direct setter so I can control when it's available.
		public Vector2 TargetPosition { get; set; } = Vector2.zero;

		public Vector2 LookDirection { get; private set; } = Vector2.down;

		private Vector2 targetLookDirection = Vector2.down;

		private void OnEnable()
		{
			TargetPosition = transform.position;
			LookDirection = Vector2.down;

			// Look & targets are only simulated for the local player. We'll just sync the look & targets from networked players.
			if (photonView.IsMine)
			{
				turnUpdateWait = new WaitForSeconds(turnTargetUpdateRate);
				StartCoroutine(UpdateTargetLookDirection());
			}
		}

		private void FixedUpdate()
		{
			if (photonView.IsMine)
			{
				LookDirection = Vector3.RotateTowards(LookDirection, targetLookDirection, turnSpeed, 0.0f);
			}
		}

		private IEnumerator UpdateTargetLookDirection()
		{
			while (isActiveAndEnabled)
			{
				targetLookDirection = (TargetPosition - (Vector2)transform.position).normalized;
				yield return turnUpdateWait;
			}
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(TargetPosition.x);
				stream.SendNext(TargetPosition.y);
				stream.SendNext(LookDirection.x);
				stream.SendNext(LookDirection.y);
			}
			else
			{
				TargetPosition.Set((float)stream.ReceiveNext(), (float)stream.ReceiveNext());
				LookDirection.Set((float)stream.ReceiveNext(), (float)stream.ReceiveNext());
			}
		}

		public void OnDrawGizmos()
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(transform.position, transform.position + (Vector3)LookDirection);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, transform.position + (Vector3)targetLookDirection);
		}
	}
}