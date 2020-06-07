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

		private bool hasPhotonObject = false;

		// This is for handling offline mode when launching straight into the scene in-editor.
		// Todo: Move this into an in-game network manager.
		public override void OnJoinedRoom() => BuildPhotonObject();
		public override void OnConnectedToMaster() => PhotonNetwork.CreateRoom("localhost");

		private void BuildPhotonObject()
		{
			if (!hasPhotonObject)
			{
				Vector3 position = localPlayer.GetComponentInChildren<Rigidbody2D>().position;
				Destroy(localPlayer);
				localPlayer = PhotonNetwork.Instantiate(playerPrefab.name, position, Quaternion.identity);
				hasPhotonObject = true;
			}
		}

		private void Start()
		{
			// This makes sure a player connecting to an existing game properly propogates their player object.
			if (PhotonNetwork.IsConnected)
			{
				BuildPhotonObject();
			}
			// This is just to let us start from the game scene and have things function locally.
			else if (!PhotonNetwork.IsConnected)
			{
				PhotonNetwork.OfflineMode = true;
			}
		}
	}
}