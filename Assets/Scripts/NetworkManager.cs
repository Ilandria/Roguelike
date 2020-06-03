using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace CCB.Roguelike
{
	public class NetworkManager : MonoBehaviourPunCallbacks
	{
		[SerializeField]
		private UnityEvent onConnectingToMaster = null;

		[SerializeField]
		private UnityEvent onConnectToMasterSuccess = null;

		[SerializeField]
		private UnityEvent onJoiningRoom = null;

		[SerializeField]
		private UnityEvent onJoinRoomSuccess = null;

		[SerializeField]
		private UnityEvent onJoinRoomFail = null;

		[SerializeField]
		private UnityEvent onCreatingRoom = null;

		[SerializeField]
		private UnityEvent onCreateRoomSuccess = null;

		[SerializeField]
		private UnityEvent onCreateRoomFail = null;

		[SerializeField]
		private UnityEvent onDisconnected = null;

		[SerializeField]
		private DebugTextRenderer debugTextRenderer = null;

		private DebugText photonStatusText = new DebugText { Name = "Network" };

		public void CreateRoom()
		{
			onCreatingRoom?.Invoke();

			if (PhotonNetwork.IsConnected)
			{
				PhotonNetwork.CreateRoom(new Guid().ToString());
			}
			else
			{
				LogPhotonError("Could not host room. Not connected to Photon master server.");
				onCreateRoomFail?.Invoke();
			}
		}

		public void JoinRandomRoom()
		{
			onJoiningRoom?.Invoke();

			if (PhotonNetwork.IsConnected)
			{
				PhotonNetwork.JoinRandomRoom();
			}
			else
			{
				LogPhotonError("Could not join room. Not connected to Photon master server.");
				onJoinRoomFail?.Invoke();
			}
		}

		private void Awake()
		{
			debugTextRenderer?.Add(photonStatusText);
		}

		private void Start()
		{
			if (!PhotonNetwork.IsConnected)
			{
				photonStatusText.Value = "Connecing to network...";
				PhotonNetwork.OfflineMode = false;
				PhotonNetwork.NickName = new Guid().ToString();
				PhotonNetwork.AutomaticallySyncScene = false;
				PhotonNetwork.GameVersion = Application.version;
				onConnectingToMaster?.Invoke();
				PhotonNetwork.ConnectUsingSettings();
			}
		}

		public override void OnJoinedRoom()
		{
			LogPhotonSuccess($"Is Host: {PhotonNetwork.IsMasterClient}, Player Count: {PhotonNetwork.CurrentRoom.PlayerCount}");
			onJoinRoomSuccess?.Invoke();
		}

		public override void OnJoinRoomFailed(short returnCode, string message)
		{
			LogPhotonError(message);
			onJoinRoomFail?.Invoke();
		}

		public override void OnJoinRandomFailed(short returnCode, string message)
		{
			LogPhotonError(message);
			onJoinRoomFail?.Invoke();
		}

		public override void OnCreatedRoom()
		{
			LogPhotonSuccess("Created room.");
			onCreateRoomSuccess?.Invoke();
		}

		public override void OnCreateRoomFailed(short returnCode, string message)
		{
			LogPhotonError(message);
			onCreateRoomFail?.Invoke();
		}

		public override void OnDisconnected(DisconnectCause cause)
		{
			LogPhotonError(cause.ToString());
			onDisconnected?.Invoke();
		}

		public override void OnConnectedToMaster()
		{
			LogPhotonSuccess("Connected to Photon master server.");
			onConnectToMasterSuccess?.Invoke();
		}

		private void LogPhotonError(string message)
		{
			photonStatusText.Colour = Color.red;
			photonStatusText.Value = message;
			Debug.LogWarning(message);
		}

		private void LogPhotonSuccess(string message)
		{
			photonStatusText.Colour = Color.white;
			photonStatusText.Value = message;
			Debug.Log(message);
		}
	}
}