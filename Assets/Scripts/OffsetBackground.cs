using UnityEngine;

namespace CCB.Roguelike
{
	public class OffsetBackground : MonoBehaviour
	{
		[SerializeField]
		private Material backgroundMaterial = null;

		[SerializeField]
		private float offsetRatio = 1.0f;

		[SerializeField]
		private Transform offsetTransform = null;

		private void Update()
		{
			backgroundMaterial.mainTextureOffset = offsetTransform.position * offsetRatio;
		}
	}
}