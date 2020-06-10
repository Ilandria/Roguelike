using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CCB.Roguelike
{
	[RequireComponent(typeof(PlayerInput))]
	public class PlayerController : MonoBehaviourPun
	{
		private static readonly Vector2 half = Vector2.one * 0.5f;

		[SerializeField]
		private Camera playerCamera = null;

		[SerializeField]
		private PlayerCharacter playerCharacter = null;

		[SerializeField]
		private Rigidbody2D playerRigidbody = null;

		[SerializeField]
		private float moveForce = 300.0f;

		[SerializeField]
		private float backstepMultiplier = 0.75f;

		/* Todo: These movement stats need to be pulled out into a movement system. Also the penalties for non-forward 
		 movement should be much less severe when walking compared to running. These values are probably good for running. */
		[SerializeField]
		private float strafeMultiplier = 0.5f;

		/// <summary>In viewport coordinates.</summary>
		public Vector2 ViewportLookPosition { get; private set; }

		private bool isUsingMouse = true;
		private Vector2 inputAxis = Vector2.zero;
		private Vector2 movementForceVector = Vector2.zero;
		private float backstepForce = 0.0f;
		private float strafeForce = 0.0f;

		public void OnMove(InputValue inputValue)
		{
			inputAxis = inputValue.Get<Vector2>();
		}

		public void OnLook(InputValue inputValue)
		{
			if (isUsingMouse)
			{
				ViewportLookPosition = playerCamera.ScreenToViewportPoint(inputValue.Get<Vector2>());
			}
			else
			{
				ViewportLookPosition = inputValue.Get<Vector2>() * 0.5f + half;
			}
		}

		private void Awake()
		{
			backstepForce = moveForce * backstepMultiplier;
			strafeForce = moveForce * strafeMultiplier;
			GetComponent<PlayerInput>().onControlsChanged += OnControlsChanged;
		}

		private void Start()
		{
			if (!photonView.IsMine)
			{
				GetComponent<PlayerInput>().onControlsChanged -= OnControlsChanged;
				Destroy(gameObject);
			}
		}

		private void OnControlsChanged(PlayerInput playerInput)
		{
			isUsingMouse = playerInput.currentControlScheme.Contains("Mouse");
		}

		private void FixedUpdate()
		{
			// Todo: This should probably be an event of some kind? It feels weird to have the controller directly apply force...
			playerRigidbody.AddForce(movementForceVector);
		}

		private void Update()
		{
			// Todo: Fire an event at some point instead of hard-linking to this. This is fine for now though.
			playerCharacter.TargetPosition = playerCamera.ViewportToWorldPoint(ViewportLookPosition);

			float lookAlignment = Vector3.Dot(playerCharacter.LookDirection, inputAxis);
			float strafeAlignment = Mathf.Abs(lookAlignment);

			float forwardSpeed = Mathf.Lerp(0.0f, moveForce, lookAlignment);
			float backstepSpeed = Mathf.Lerp(0.0f, backstepForce, -lookAlignment);
			float strafeSpeed = Mathf.Lerp(strafeForce, 0.0f, strafeAlignment);

			movementForceVector = inputAxis * (forwardSpeed + backstepSpeed + strafeSpeed);
		}
	}
}