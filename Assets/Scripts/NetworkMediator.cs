using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace CCB.Roguelike
{
	public class NetworkMediator : MonoBehaviourPunCallbacks
	{
		[SerializeField]
		private PlatformMediator platformMediator = null;

		[SerializeField]
		private UnityEvent onJoinedRoom = null;

		[SerializeField]
		private UnityEvent onConnectedToMaster = null;

		/// <summary>
		/// Raised when Start is called and Photon is already connected.
		/// </summary>
		[SerializeField]
		private UnityEvent onAlreadyConnectedToMaster = null;

		[SerializeField]
		private UnityEvent onDisconnected = null;

		public void CreateGame()
		{
			if (PhotonNetwork.IsConnected && !PhotonNetwork.InRoom)
			{
				// Todo: Probably need to move this stuff out to another class...
				/*if (platformMediator.CreateLobby().HasValue)
				{
					Debug.Log("Created steam lobby");
				}*/

				// Todo: Passwords, friends only, single player, etc.
				PhotonNetwork.CreateRoom(Guid.NewGuid().ToString());
			}
		}

		public void JoinGame(string gameName)
		{
			if (PhotonNetwork.IsConnected && !PhotonNetwork.InRoom)
			{
				// Todo: Passwords, friends only, single player, etc.
				PhotonNetwork.JoinRoom(gameName);
			}
		}

		public override void OnJoinedRoom()
		{
			onJoinedRoom?.Invoke();
		}

		public override void OnConnectedToMaster()
		{
			onConnectedToMaster?.Invoke();
		}

		public override void OnDisconnected(DisconnectCause cause)
		{
			// Todo: Handle reconnections.
			onDisconnected?.Invoke();
		}

		private void Start()
		{
			if (!PhotonNetwork.IsConnected)
			{
				PhotonNetwork.NickName = platformMediator.Name;
				PhotonNetwork.AutomaticallySyncScene = false;
				PhotonNetwork.GameVersion = Application.version;
				PhotonNetwork.ConnectUsingSettings();
				// Todo: Allow for offline play using PhotonNetwork.OfflineMode = true? Player must be online
				// on the platform (Steam) though so this may not make sense.
			}
			else
			{
				onAlreadyConnectedToMaster?.Invoke();
			}
		}
	}
}