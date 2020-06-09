using Photon.Pun;
using UnityEngine;

namespace CCB.Roguelike
{
	public class PlayerFactory : MonoBehaviour
	{
		[SerializeField]
		private GameObject playerPrefab = null;

		private GameObject localPlayer = null;

		public void RebuildPhotonObject()
		{
			Vector3 position = Vector3.zero;

			if (localPlayer != null)
			{
				position = localPlayer.GetComponentInChildren<Rigidbody2D>().position;
				PhotonNetwork.Destroy(localPlayer);
			}

			localPlayer = PhotonNetwork.Instantiate(playerPrefab.name, position, Quaternion.identity);
		}
	}
}