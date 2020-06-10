using System;
using System.Collections;
using UnityEngine;

namespace CCB.Roguelike
{
	public class VisionFog : MonoBehaviour
	{
		[SerializeField]
		private PlayerCharacter playerCharacter = null;

		[SerializeField]
		private GameObject visionSource = null;

		[SerializeField]
		private int rayCount = 720;

		[SerializeField]
		private float visionUpdateRate = 0.1f;

		// Todo: Move this into PlayerCharacter.
		[SerializeField]
		private float fov = 220.0f;

		// Todo: Move this into PlayerCharacter.
		[SerializeField]
		private float viewDistance = 10.0f;

		// Todo: Move this into PlayerCharacter.
		[SerializeField]
		private float minViewDistance = 0.5f;

		private readonly float deg2Rad = Mathf.PI / 180.0f;
		private Mesh visionMesh = null;
		private Vector3[] vertices = null;
		private Vector2[] uv = null;
		private int[] indices = null;
		private RaycastHit2D[] rayHitArray = new RaycastHit2D[1] { new RaycastHit2D() };
		private int solidObjectLayer = -1;
		private float angle = 0.0f;
		private float angleIncrease = 0.0f;
		private Vector3 origin = Vector3.zero;
		private Vector2 rayVector = Vector2.zero;
		private Vector3 localRayVector = Vector3.zero;
		private WaitForSeconds visionUpdateWait = null;
		private float fovRadians = 0.0f;
		private float halfFovRadians = 0.0f;
		private float solidObjectViewFactor = 0.0f;
		
		public void OnEnable()
		{
			visionMesh = new Mesh();
			GetComponent<MeshFilter>().mesh = visionMesh;

			solidObjectLayer = LayerMask.GetMask("Solid Objects"); // Todo: Don't hardcode this.

			angleIncrease = Mathf.PI * 2.0f / rayCount;
			int numVertices = rayCount + 1; // +1 to account for the center vertex.
			vertices = new Vector3[numVertices];
			uv = new Vector2[numVertices];
			indices = new int[rayCount * 3];

			for (int ray = 0; ray < rayCount; ray++)
			{
				int tri = ray * 3;
				indices[tri + 0] = 0;
				indices[tri + 1] = ray + 1;
				indices[tri + 2] = ray + 2 < numVertices ? ray + 2 : 1;
			}

			visionMesh.SetVertices(vertices);
			visionMesh.SetUVs(0, uv);
			visionMesh.SetIndices(indices, MeshTopology.Triangles, 0);

			ReconfigureVisionMesh();
			visionUpdateWait = new WaitForSeconds(visionUpdateRate);
			StartCoroutine(UpdateVision());
		}

		// Todo: Use this once any time the player's vision needs to fundamentally change.
		public void ReconfigureVisionMesh()
		{
			fovRadians = fov * deg2Rad;
			halfFovRadians = fovRadians / 2.0f;
			solidObjectViewFactor = 1.0f - 1.0f / (minViewDistance + 1.0f); // Todo: Don't hardcode this. Approaches 1 at x => inf., 0.5 at x = 1.
		}

		public IEnumerator UpdateVision()
		{
			while (isActiveAndEnabled)
			{
				// Turns Vector3.Angle into a 0-360 range to properly offset vision cone.
				float lookAngle = Vector3.Angle(Vector3.right, playerCharacter.LookDirection);
				if (Vector3.Dot(Vector3.up, playerCharacter.LookDirection) < 0.0f)
				{
					lookAngle = 360.0f - lookAngle;
				}

				origin = visionSource.transform.position;
				angle = halfFovRadians + lookAngle * deg2Rad;
				Vector3 localOrigin = transform.InverseTransformPoint(origin);
				vertices[0] = localOrigin;
				float totalAngle = 0.0f;

				for (int i = 1; i <= rayCount; i++)
				{
					rayVector.Set(Mathf.Cos(angle), Mathf.Sin(angle));
					localRayVector = transform.InverseTransformDirection(rayVector);

					// We can see farther in the forward view arc.
					float effectiveViewDistance = totalAngle <= fovRadians ? viewDistance : minViewDistance;

					// Vision ray hit nothing.
					if (Physics2D.RaycastNonAlloc(origin, rayVector, rayHitArray, effectiveViewDistance, solidObjectLayer) == 0)
					{
						vertices[i] = localOrigin + localRayVector * effectiveViewDistance;
					}
					// Vision ray hit something.
					else
					{
						/* Todo: Add a way to toggle the ability to see through walls. This should  add some kind of view distance penalty that's
						 * configurable between "no penalty" and "normal vision limited by walls". */
						// Distance from the player to the maximum depth within the solid object that the player could see.
						float viewBlockDistance = Vector3.Distance(rayHitArray[0].point + rayVector * solidObjectViewFactor, origin);
						// To prevent seeing through walls, either use the effective view distance or the point within the solid object, whichever is shorter.
						vertices[i] = localOrigin + localRayVector * Mathf.Min(effectiveViewDistance, viewBlockDistance);
					}

					totalAngle += angleIncrease;
					angle -= angleIncrease;
				}

				visionMesh.SetVertices(vertices);

				yield return visionUpdateWait;
			}
		}
	}
}