using UnityEngine;
using UnityEngine.InputSystem;

namespace CCB.Roguelike
{
	[RequireComponent(typeof(PlayerInput))]
	public class PlayerController : MonoBehaviour
	{
		private static readonly Vector2 half = Vector2.one * 0.5f;

		[SerializeField]
		private Camera playerCamera = null;

		[SerializeField]
		private Rigidbody2D playerRigidbody = null;

		[SerializeField]
		private float moveForce = 0.0f;

		[SerializeField]
		private Vector2 inputAxis = Vector2.zero;

		[SerializeField]
		private Vector2 lookPosition = half;
		/// <summary>In viewport coordinates.</summary>
		public Vector2 LookPosition => lookPosition;

		[SerializeField]
		private bool isUsingMouse = true;

		public void OnMove(InputValue inputValue)
		{
			inputAxis = inputValue.Get<Vector2>();
		}

		public void OnLook(InputValue inputValue)
		{
			if (isUsingMouse)
			{
				lookPosition = playerCamera.ScreenToViewportPoint(inputValue.Get<Vector2>());
			}
			else
			{
				lookPosition = inputValue.Get<Vector2>() * 0.5f + half;
			}
		}

		private void Awake()
		{
			GetComponent<PlayerInput>().onControlsChanged += OnControlsChanged;
		}

		private void OnControlsChanged(PlayerInput playerInput)
		{
			isUsingMouse = playerInput.currentControlScheme.Contains("Mouse");
		}

		private void FixedUpdate()
		{
			playerRigidbody.AddForce(inputAxis * moveForce);
		}
	}
}