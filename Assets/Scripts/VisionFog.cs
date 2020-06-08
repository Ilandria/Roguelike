﻿using System;
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
		private float wallVisionDepth = 0.2f;

		// Todo: Move this into PlayerCharacter.
		[SerializeField]
		private float fov = 220.0f;

		// Todo: Move this into PlayerCharacter.
		[SerializeField]
		private float viewDistance = 10.0f;

		// Todo: Move this into PlayerCharacter.
		[SerializeField]
		private float minimumViewDistance = 0.5f;

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
		
		public void OnEnable()
		{
			visionMesh = new Mesh();
			GetComponent<MeshFilter>().mesh = visionMesh;

			fovRadians = fov * (Mathf.PI / 180.0f);
			halfFovRadians = fovRadians / 2.0f;
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
				angle = halfFovRadians + lookAngle * (Mathf.PI / 180.0f);
				Vector3 originPoint = transform.InverseTransformPoint(origin);
				vertices[0] = originPoint;
				float totalAngle = 0.0f;

				for (int i = 1; i <= rayCount; i++)
				{
					rayVector.Set(Mathf.Cos(angle), Mathf.Sin(angle));
					localRayVector = transform.InverseTransformDirection(rayVector);
					bool inForwardArc = totalAngle <= fovRadians;

					// Todo: Move the vision start buffer thing (* 0.45f) somewhere else not hard-coded.
					if (inForwardArc)
					{
						// Hit nothing in the forward view arc.
						if (Physics2D.RaycastNonAlloc(origin, rayVector, rayHitArray, viewDistance, solidObjectLayer) == 0)
						{
							vertices[i] = originPoint + localRayVector * viewDistance;
						}
						// Hit something in the forward view arc.
						else
						{
							// Todo: Instead of the padding here, have vision go until it is no longer hitting a collider.
							vertices[i] = transform.InverseTransformPoint(rayHitArray[0].point) + localRayVector * wallVisionDepth;
						}
					}
					// In the rear view arc.
					else
					{
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