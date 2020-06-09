using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace CCB.Roguelike
{
	public class InGameNetworkCoordinator : MonoBehaviourPunCallbacks
	{
		[SerializeField]
		private NetworkConnectionInfo connectionInfo = null;

		[SerializeField]
		private UnityEvent onJoinRoom = null;

		public override void OnJoinedRoom()
		{
			connectionInfo.connectionState = NetworkConnectionState.ConnectedToRoom;
			onJoinRoom?.Invoke();
		}

		// This is for handling offline mode when launching straight into the game scene in-editor.
		public override void OnConnectedToMaster()
		{
			connectionInfo.connectionState = NetworkConnectionState.CreatingRoom;
			PhotonNetwork.CreateRoom(connectionInfo.roomName);
		}

		private void Start()
		{
			// This is just to let us start from the game scene and have things function locally.
			if (!PhotonNetwork.IsConnected)
			{
				connectionInfo.connectionState = NetworkConnectionState.ConnectingToMaster;
				connectionInfo.roomName = "localhost";
				PhotonNetwork.OfflineMode = true;
			}
			else
			{
				// Todo: Handle all failure cases (go back to menu if can't connect, etc.). This is currently happy-paths only.
				// Todo: Turn these into something like command.Run instead of a switch.
				switch(connectionInfo.connectionState)
				{
					case NetworkConnectionState.CreatingRoom:
						PhotonNetwork.CreateRoom(connectionInfo.roomName);
						break;

					case NetworkConnectionState.JoiningRoom:
						PhotonNetwork.JoinRoom(connectionInfo.roomName);
						break;

					default:
						Debug.LogError("Trying to start game in online mode but neither creating nor joining a room.");
						break;
				}
			}
		}
	}
}