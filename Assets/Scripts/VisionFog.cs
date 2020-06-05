using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

namespace CCB.Roguelike
{
	public class VisionFog : MonoBehaviour
	{
		// Todo: Turn this into a list of vision sources once lighting system is ready. Need vision ranges.
		private readonly HashSet<GameObject> visionSources = new HashSet<GameObject>();

		// Todo: Turn this into a list of light sources once lighting system is ready. Need light ranges.
		private readonly HashSet<GameObject> lightSources = new HashSet<GameObject>();

		public void AddVisionSource(GameObject visionSource)
		{

		}

		public void RemoveVisionSource(GameObject visionSource)
		{

		}

		public void AddLightSource(GameObject lightSource)
		{

		}

		public void RemoveLightSource(GameObject lightSource)
		{

		}

		private Mesh temp = null;
		[SerializeField] private GameObject tempPlayer = null;
		public void Start()
		{
			temp = new Mesh();
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = temp;
		}

		public void Update()
		{
			double fov = 210.0 * (Math.PI / 180.0);
			Vector3 origin = tempPlayer.transform.position;
			int rayCount = 420;
			float angle = (float)(fov / 2.0);
			float angleIncrease = (float)(fov / rayCount);
			float viewDistance = 6.0f;

			Vector3[] vertices = new Vector3[rayCount + 2];
			Vector2[] uv = new Vector2[vertices.Length];
			int[] indices = new int[rayCount * 3 + 3];

			vertices[0] = origin;

			int vertex = 1;
			int triangle = 0;
			int solidObjectLayer = LayerMask.GetMask("Solid Objects");
			for (int i = 0; i <= rayCount; i++)
			{
				Vector2 rayVector = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
				// Todo: Switch to this: Physics2D.RaycastNonAlloc
				RaycastHit2D rayHit = Physics2D.Raycast(origin, rayVector, viewDistance, solidObjectLayer);
				if (rayHit.collider == null)
				{
					vertices[vertex] = origin + (Vector3)rayVector * viewDistance;
				}
				else
				{
					vertices[vertex] = rayHit.point + rayVector * 0.25f;
				}

				if (i > 0)
				{
					indices[triangle + 0] = 0;
					indices[triangle + 1] = vertex - 1;
					indices[triangle + 2] = vertex;
					triangle += 3;
				}
				vertex++;
				angle -= angleIncrease;
			}

			temp.vertices = vertices;
			temp.uv = uv;
			temp.triangles = indices;
		}
	}
}