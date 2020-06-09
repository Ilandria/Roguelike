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
		private float minimumViewDistance = 0.5f;

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
			solidObjectViewFactor = 1.0f - 1.0f / (minimumViewDistance + 1.0f); // Todo: Don't hardcode this. Approaches 1 at x => inf., 0.5 at x = 1.
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
				Vector3 originPoint = transform.InverseTransformPoint(origin);
				vertices[0] = originPoint;
				float totalAngle = 0.0f;

				for (int i = 1; i <= rayCount; i++)
				{
					rayVector.Set(Mathf.Cos(angle), Mathf.Sin(angle));
					localRayVector = transform.InverseTransformDirection(rayVector);
					bool inForwardArc = totalAngle <= fovRadians;

					if (inForwardArc)
					{
						// Todo: Remove all of this minimumViewDistance stuff from here after adding a circle stencil mesh to depict that.
						// Hit nothing in the forward view arc.
						if (Physics2D.RaycastNonAlloc(origin + localRayVector * minimumViewDistance, rayVector, rayHitArray, viewDistance - minimumViewDistance, solidObjectLayer) == 0)
						{
							vertices[i] = originPoint + localRayVector * viewDistance;
						}
						// Hit something in the forward view arc.
						else
						{
							// Todo: Find a better way to allow the player to see slightly into solid objects (just so they can see the object). Disable stencil mask while looking at, maybe?
							vertices[i] = transform.InverseTransformPoint(rayHitArray[0].point);// + localRayVector * solidObjectViewFactor;
						}
					}
					// In the rear view arc.
					else
					{
						// Todo: Instead of having a full 360 mesh generated, just use a 2nd mesh (a circle) with stencil writing shader to depict this. Will speed up performance and make math easier.
						vertices[i] = originPoint + localRayVector * minimumViewDistance;
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