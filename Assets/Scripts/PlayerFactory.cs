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

		private bool isPhotonObject = false;

		// This is for handling offline mode when launching straight into the scene in-editor.
		public override void OnJoinedRoom()
		{
			RebuildPlayerObject();
		}

		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			RebuildPlayerObject();
		}

		private void RebuildPlayerObject()
		{
			Vector3 position = localPlayer.GetComponentInChildren<Rigidbody2D>().position;

			if (isPhotonObject)
			{
				PhotonNetwork.Destroy(localPlayer);
			}
			else
			{
				Destroy(localPlayer);
			}

			localPlayer = PhotonNetwork.Instantiate(playerPrefab.name, position, Quaternion.identity);
			isPhotonObject = true;
		}

		// This is just here so we can launch from the game scene in offline mode.
		// Todo: Move this into an in-game network manager.
		public override void OnConnectedToMaster()
		{
			PhotonNetwork.CreateRoom("localhost");
		}

		private void Awake()
		{
			// This makes sure a player connecting to an existing game properly propogates their player object.
			if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
			{
				RebuildPlayerObject();
			}
		}

		private void Start()
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