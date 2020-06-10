using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace CCB.Roguelike
{
	public class PlayerCharacter : MonoBehaviourPun, IPunObservable
	{
		[SerializeField]
		private Transform eyeTransform = null;

		// Todo: This needs to be hooked up to character stats and not hard-coded.
		[SerializeField]
		[Range(0.0f, 0.25f)]
		private float turnSpeed = 0.5f;

		// Todo: This should be handled by some under-the-hood config file.
		[SerializeField]
		private float turnTargetUpdateRate = 0.25f;
		private WaitForSeconds turnUpdateWait = null;

		private Vector2 targetPosition = Vector2.zero;
		public Vector2 TargetPosition
		{
			get => targetPosition;
			set
			{
				targetPosition.Set(value.x, value.y);
			}
		}

		private Vector2 lookDirection = Vector2.down;
		public Vector2 LookDirection => lookDirection;

		private Vector2 targetLookDirection = Vector2.down;

		private void OnEnable()
		{
			TargetPosition = transform.position;
			lookDirection = Vector2.down;

			// Look & targets are only simulated for the local player. We'll just sync the look & targets from networked players.
			if (photonView.IsMine)
			{
				turnUpdateWait = new WaitForSeconds(turnTargetUpdateRate);
				StartCoroutine(UpdateTargetLookDirection());
			}
		}

		private void FixedUpdate()
		{
			// Other players have their LookDirection set directly in OnPhotonSerializeView.
			if (photonView.IsMine)
			{
				lookDirection = Vector3.RotateTowards(LookDirection, targetLookDirection, turnSpeed, 0.0f);
			}
		}

		private IEnumerator UpdateTargetLookDirection()
		{
			// This coroutine is only started for the local photon view, so we don't need to check that (see OnEnable).
			while (isActiveAndEnabled)
			{
				targetLookDirection = (TargetPosition - (Vector2)eyeTransform.position).normalized;
				yield return turnUpdateWait;
			}
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting && photonView.IsMine)
			{
				stream.SendNext(TargetPosition.x);
				stream.SendNext(TargetPosition.y);
				stream.SendNext(LookDirection.x);
				stream.SendNext(LookDirection.y);
			}
			else if (!photonView.IsMine)
			{
				TargetPosition.Set((float)stream.ReceiveNext(), (float)stream.ReceiveNext());
				lookDirection.Set((float)stream.ReceiveNext(), (float)stream.ReceiveNext());
			}
		}
	}
}