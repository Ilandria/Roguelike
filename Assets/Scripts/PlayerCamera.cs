using Photon.Pun;
using UnityEngine;

namespace CCB.Roguelike
{
	[RequireComponent(typeof(Camera))]
	public class PlayerCamera : MonoBehaviourPun
	{
		[SerializeField]
		private PlayerController playerController = null;

		[SerializeField]
		private Rigidbody2D trackTarget = null;

		[SerializeField]
		private float velocityFactor = 0.0f;

		[SerializeField]
		private float lookFactor = 0.0f;

		[SerializeField]
		[Range(0.0f, 1.0f)]
		private float smoothing = 0.0f;

		[SerializeField]
		private Vector2 maxOffset = Vector2.zero;

		private Vector3 targetPosition = Vector3.zero;
		private Vector2 lookOffset = Vector2.zero;
		private Vector2 targetOffset = Vector3.zero;
		private Camera playerCamera = null;
		private float cameraZ = 0.0f;
		private float lerpFactor = 1.0f;

		private void OnEnable()
		{
			playerCamera = GetComponent<Camera>();
			cameraZ = playerCamera.transform.position.z;
			playerCamera.transform.position = new Vector3
			(
				trackTarget.position.x,
				trackTarget.position.y,
				cameraZ
			);
			targetPosition = playerCamera.transform.position;
			lerpFactor = 1.0f - smoothing;
		}

		private void FixedUpdate()
		{
			lookOffset = (Vector2)playerCamera.ViewportToWorldPoint(playerController.ViewportLookPosition) - trackTarget.position;
			targetOffset = trackTarget.velocity * velocityFactor + lookOffset * lookFactor;
			targetOffset.Set(Mathf.Clamp(targetOffset.x, -maxOffset.x, maxOffset.x), Mathf.Clamp(targetOffset.y, -maxOffset.y, maxOffset.y));
			targetPosition.Set(trackTarget.position.x + targetOffset.x, trackTarget.position.y + targetOffset.y, cameraZ);
			targetPosition = Vector3.Lerp(playerCamera.transform.position, targetPosition, lerpFactor);
		}

		private void LateUpdate()
		{
			playerCamera.transform.position = targetPosition;
		}

		private void Start()
		{
			if (!photonView.IsMine)
			{
				Destroy(gameObject);
			}
		}
	}
}