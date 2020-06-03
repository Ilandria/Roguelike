using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace CCB.Roguelike
{
	public class PlayerFactory : MonoBehaviourPunCallbacks
	{
		[SerializeField]
		private GameObject playerPrefab = null;

		[SerializeField]
		private GameObject localPlayer = null;

		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			// Todo: Figure out why photon won't enter this room in offline mode.
			Debug.Log("Player entering room");
			Vector3 position = Vector3.zero;
			Quaternion rotation = Quaternion.identity;

			if (localPlayer != null)
			{
				position = localPlayer.transform.position;
				rotation = localPlayer.transform.rotation;

				PhotonNetwork.Destroy(localPlayer);
			}

			localPlayer = PhotonNetwork.Instantiate(playerPrefab.name, position, rotation);
		}

		// This is just here so we can launch from the game scene in offline mode.
		// Todo: Move this into an in-game network manager.
		public override void OnConnectedToMaster()
		{
			Debug.Log("Connected to offline master.");
			PhotonNetwork.CreateRoom(string.Empty);
		}

		private void Awake()
		{
			// This is just to let us start from the game scene and have things function locally.
			// Todo: Move this into an in-game network manager.
			if (!PhotonNetwork.IsConnected)
			{
				PhotonNetwork.OfflineMode = true;
			}
		}
	}
}