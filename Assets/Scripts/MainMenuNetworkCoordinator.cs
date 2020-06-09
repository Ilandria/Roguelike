using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace CCB.Roguelike
{
	public class MainMenuNetworkCoordinator : MonoBehaviourPunCallbacks
	{
		// Todo: Guess what? Don't hardcode the room name.
		[SerializeField]
		private string roomName = "DON'T HARDCODE THIS";

		[SerializeField]
		private NetworkConnectionInfo connectionInfo = null;

		[SerializeField]
		private UnityEvent onConnectingToMaster = null;

		[SerializeField]
		private UnityEvent onConnectToMasterSuccess = null;

		[SerializeField]
		private UnityEvent onDisconnected = null;

		/// <summary>
		/// Set the connection info to flag that a room should be created after loading the game scene.
		/// </summary>
		public void PrepareToCreateRoom()
		{
			// Todo: This should pass in game name, load game, etc.
			connectionInfo.roomName = roomName;
			connectionInfo.connectionState = NetworkConnectionState.CreatingRoom;
		}

		/// <summary>
		/// Set the connection info to flag that a room should be joined after loading the game scene.
		/// </summary>
		public void PrepareToJoinRoom()
		{
			// Todo: Allow for selecting a room by name down the road.
			connectionInfo.roomName = roomName;
			connectionInfo.connectionState = NetworkConnectionState.JoiningRoom;
		}

		private void Start()
		{
			connectionInfo.roomName = string.Empty;
			connectionInfo.connectionState = NetworkConnectionState.DisconnectedFromMaster;
			ConnectToMaster();
		}

		public override void OnDisconnected(DisconnectCause cause)
		{
			connectionInfo.roomName = string.Empty;
			connectionInfo.connectionState = NetworkConnectionState.DisconnectedFromMaster;
			onDisconnected?.Invoke();

			// Todo: Put this into a coroutine that attempts to connect a few times and then goes into offline mode.
			ConnectToMaster();
		}

		public override void OnConnectedToMaster()
		{
			connectionInfo.roomName = string.Empty;
			connectionInfo.connectionState = NetworkConnectionState.ConnectedToMaster;
			onConnectToMasterSuccess?.Invoke();
		}

		private void ConnectToMaster()
		{
			if (!PhotonNetwork.IsConnected)
			{
				connectionInfo.roomName = string.Empty;
				connectionInfo.connectionState = NetworkConnectionState.ConnectingToMaster;
				PhotonNetwork.OfflineMode = false;
				PhotonNetwork.NickName = new Guid().ToString();
				PhotonNetwork.AutomaticallySyncScene = false;
				PhotonNetwork.GameVersion = Application.version;
				onConnectingToMaster?.Invoke();
				PhotonNetwork.ConnectUsingSettings();
			}
		}
	}
}