using System.Collections;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace CCB.Roguelike
{
	public class VisionMesh : MonoBehaviour
	{
		[SerializeField]
		private PlayerCharacter playerCharacter = null;

		[SerializeField]
		private GameObject visionSource = null;

		[SerializeField]
		private int rayCount = 90;

		[SerializeField]
		private float visionUpdateRate = 0.2f;

		// Todo: Move this into PlayerCharacter.
		[SerializeField]
		private float fov = 220.0f;

		// Todo: Move this into PlayerCharacter.
		[SerializeField]
		private float viewDistance = 10.0f;

		// Todo: Move this into PlayerCharacter.
		[SerializeField]
		private float minViewDistance = 0.6f;

		private NativeArray<float2> vertices;
		private readonly float deg2Rad = Mathf.PI / 180.0f;
		private RaycastHit2D[] rayHitArray = new RaycastHit2D[1] { new RaycastHit2D() };
		private int solidObjectLayer = -1;
		private float angleIncrease = 0.0f;
		private Vector2 rayHeading = Vector2.zero;
		private WaitForSeconds visionUpdateWait = null;
		
		public void OnEnable()
		{
			solidObjectLayer = LayerMask.GetMask("Solid Objects"); // Todo: Don't hardcode this.

			angleIncrease = Mathf.PI * 2.0f / rayCount;

			if (vertices.IsCreated)
			{
				vertices.Dispose();
			}

			vertices = new NativeArray<float2>(rayCount + 1, Allocator.Persistent);

			visionUpdateWait = new WaitForSeconds(visionUpdateRate);
			StartCoroutine(UpdateVision());
		}

		private void OnDisable()
		{
			if (vertices.IsCreated)
			{
				vertices.Dispose();
			}
		}

		public IEnumerator UpdateVision()
		{
			float fovRadians = fov * deg2Rad;
			float halfFovRadians = fovRadians / 2.0f;

			while (isActiveAndEnabled)
			{
				// Turns Vector3.Angle into a 0-360 range to properly offset vision cone.
				float lookAngle = Vector3.Angle(Vector3.right, playerCharacter.LookDirection);
				if (Vector3.Dot(Vector3.up, playerCharacter.LookDirection) < 0.0f)
				{
					lookAngle = 360.0f - lookAngle;
				}

				Vector2 origin = visionSource.transform.position;
				vertices[0] = new float2(origin.x, origin.y);
				float angle = halfFovRadians + lookAngle * deg2Rad;
				float totalAngle = 0.0f;

				for (int i = 1; i <= rayCount; i++)
				{
					rayHeading.Set(Mathf.Cos(angle), Mathf.Sin(angle));

					// We can see farther in the forward view arc.
					float unobstructedViewDistance = totalAngle <= fovRadians ? viewDistance : minViewDistance;

					// Vision ray hit nothing.
					if (Physics2D.RaycastNonAlloc(origin, rayHeading, rayHitArray, unobstructedViewDistance, solidObjectLayer) == 0)
					{
						vertices[i] = origin + rayHeading * unobstructedViewDistance;
					}
					// Vision ray hit something.
					else
					{
						vertices[i] = rayHitArray[0].point;
					}

					totalAngle += angleIncrease;
					angle -= angleIncrease;
				}

				yield return visionUpdateWait;
			}
		}

		private void OnDrawGizmos()
		{
			if (vertices.IsCreated)
			{
				Vector3 start = new Vector3(vertices[0].x, vertices[0].y, 0);
				Gizmos.color = Color.cyan;
				for (int i = 1; i < vertices.Length; i++)
				{
					Gizmos.DrawLine(start, new Vector3(vertices[i].x, vertices[i].y, 0));
				}
			}
		}
	}
}