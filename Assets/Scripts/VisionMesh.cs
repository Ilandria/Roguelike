using System.Collections;
using UnityEngine;

namespace CCB.Roguelike
{
	public class VisionMesh : MonoBehaviour
	{
		[SerializeField]
		private PlayerCharacter playerCharacter = null;

		[SerializeField]
		private Transform visionSource = null;

		[SerializeField]
		private int rayCount = 720;

		[SerializeField]
		private float visionUpdateRate = 0.1f;

		// Todo: Move this into PlayerCharacter.
		[SerializeField]
		[Range(0.0f, 360.0f)]
		private float fov = 220.0f;

		// Todo: Move this into PlayerCharacter.
		[SerializeField]
		private float viewDistance = 10.0f;

		// Todo: Move this into PlayerCharacter.
		[SerializeField]
		private float minViewDistance = 0.5f;

		private Mesh visionMesh = null;
		private Vector3[] vertices = null;
		private Vector2[] uv = null;
		private int[] indices = null;
		private RaycastHit2D[] rayHitArray = new RaycastHit2D[1] { new RaycastHit2D() };
		private int solidObjectLayer = -1;
		private WaitForSeconds visionUpdateWait = null;
		private Vector2[] rayHeadings = null;
		private float[] rayAngles = null;
		
		public void OnEnable()
		{
			visionMesh = new Mesh();
			GetComponent<MeshFilter>().mesh = visionMesh;
			solidObjectLayer = LayerMask.GetMask("Solid Objects"); // Todo: Don't hardcode this.

			int numVertices = rayCount + 1; // +1 to account for the center vertex.
			vertices = new Vector3[numVertices];
			uv = new Vector2[numVertices];
			indices = new int[rayCount * 3];

			float angleIncrease = Mathf.PI * 2.0f / rayCount;
			rayHeadings = new Vector2[rayCount];
			rayAngles = new float[rayCount];

			for (int ray = 0; ray < rayCount; ray++)
			{
				int tri = ray * 3;
				indices[tri + 0] = 0;
				indices[tri + 2] = ray + 1;
				indices[tri + 1] = ray + 2 < numVertices ? ray + 2 : 1;

				rayAngles[ray] = ray * angleIncrease;
				rayHeadings[ray] = new Vector2(Mathf.Cos(rayAngles[ray]), Mathf.Sin(rayAngles[ray]));
			}

			visionMesh.SetVertices(vertices);
			visionMesh.SetUVs(0, uv);
			visionMesh.SetIndices(indices, MeshTopology.Triangles, 0);

			visionUpdateWait = new WaitForSeconds(visionUpdateRate);
			StartCoroutine(UpdateVision());
		}

		public IEnumerator UpdateVision()
		{
			while (isActiveAndEnabled)
			{
				float solidObjectViewFactor = 1.0f - 1.0f / (minViewDistance + 1.0f); // Todo: Don't hardcode this. Approaches 1 at x => inf., 0.5 at x = 1.
				Vector3 worldOrigin = visionSource.position;
				Vector3 localOrigin = transform.InverseTransformPoint(worldOrigin);
				vertices[0] = localOrigin;

				for (int i = 1; i <= rayCount; i++)
				{
					Vector2 rayHeading = rayHeadings[i - 1];

					// We can see farther in the forward view arc.
					float viewAlignment = Vector3.Dot(rayHeading, playerCharacter.LookDirection) * 0.5f + 0.5f;
					float fovEdgeLocation = 1.0f - (fov / 360.0f);
					viewAlignment = 1.0f - Mathf.Clamp01((fovEdgeLocation - viewAlignment) / fovEdgeLocation);
					float unobstructedViewDistance = Mathf.Lerp(minViewDistance, viewDistance, Mathf.Pow(viewAlignment, 10.0f));

					// Vision ray hit nothing.
					if (Physics2D.RaycastNonAlloc(worldOrigin, rayHeading, rayHitArray, unobstructedViewDistance, solidObjectLayer) == 0)
					{
						vertices[i] = localOrigin + (Vector3)rayHeading * unobstructedViewDistance;
					}
					// Vision ray hit something.
					else
					{
						/* Todo: Add a way to toggle the ability to see through walls. This should add some kind of view distance penalty that's
						 * configurable between "no penalty" and "normal vision limited by walls". */
						// Distance from the player to the maximum depth within the solid object that the player could see.
						float obstructedViewDistance = Vector3.Distance(rayHitArray[0].point + rayHeading * solidObjectViewFactor, worldOrigin);
						// To prevent seeing through walls, either use the effective view distance or the point within the solid object, whichever is shorter.
						vertices[i] = localOrigin + (Vector3)rayHeading * Mathf.Min(unobstructedViewDistance, obstructedViewDistance);
					}
				}

				visionMesh.SetVertices(vertices);
				yield return visionUpdateWait;
			}
		}
	}
}