using UnityEngine;
using UnityEngine.InputSystem;

namespace CCB.Roguelike
{
	[RequireComponent(typeof(PlayerInput))]
	public class PlayerController : MonoBehaviour
	{
		[SerializeField]
		private Camera playerCamera = null;

		[SerializeField]
		private Rigidbody2D playerRigidbody = null;

		[SerializeField]
		private float moveForce = 0.0f;

		[SerializeField]
		private Vector2 inputAxis = Vector2.zero;

		[SerializeField]
		private Vector2 lookPosition = Vector2.zero;
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
				lookPosition = playerCamera.ScreenToWorldPoint(inputValue.Get<Vector2>());
			}
			else
			{
				lookPosition = playerRigidbody.position + inputValue.Get<Vector2>();
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