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
		private int rayCount = 630;

		[SerializeField]
		private float visionUpdateRate = 0.1f;

		// Todo: Move this into PlayerCharacter.
		[SerializeField]
		private float wallVisionDepth = 0.2f;

		// Todo: Move this into PlayerCharacter.
		[SerializeField]
		private double fov = 210.0 * (Math.PI / 180.0);

		// Todo: Move this into PlayerCharacter.
		[SerializeField]
		private float viewDistance = 10.0f;

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
		
		public void OnEnable()
		{
			visionMesh = new Mesh();
			GetComponent<MeshFilter>().mesh = visionMesh;

			solidObjectLayer = LayerMask.GetMask("Solid Objects"); // Todo: Don't hardcode this.
			angleIncrease = (float)(fov / rayCount);
			vertices = new Vector3[rayCount + 2];
			uv = new Vector2[vertices.Length];
			indices = new int[rayCount * 3 + 3];

			int triangle = 0;

			for (int vertex = 1; vertex <= rayCount; vertex++)
			{
				indices[triangle + 0] = 0;
				indices[triangle + 1] = vertex - 1;
				indices[triangle + 2] = vertex;
				triangle += 3;
			}

			visionMesh.vertices = vertices;
			visionMesh.uv = uv;
			visionMesh.triangles = indices;

			visionUpdateWait = new WaitForSeconds(visionUpdateRate);
			StartCoroutine(UpdateVision());
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
				angle = (float)(fov / 2.0) + lookAngle * ((float)Math.PI / 180.0f);
				vertices[0] = transform.InverseTransformPoint(origin);
				int vertex = 1;

				for (int i = 0; i <= rayCount; i++)
				{
					rayVector.Set(Mathf.Cos(angle), Mathf.Sin(angle));
					localRayVector = transform.InverseTransformDirection(rayVector);

					if (Physics2D.RaycastNonAlloc(origin, rayVector, rayHitArray, viewDistance, solidObjectLayer) == 0)
					{
						vertices[vertex] = transform.InverseTransformPoint(origin) + localRayVector * viewDistance;
					}
					else
					{
						vertices[vertex] = transform.InverseTransformPoint(rayHitArray[0].point) + localRayVector * wallVisionDepth;
					}

					vertex++;
					angle -= angleIncrease;
				}

				visionMesh.vertices = vertices;

				yield return visionUpdateWait;
			}
		}
	}
}