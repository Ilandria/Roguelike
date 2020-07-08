using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CCB.Roguelike
{
	// Todo: Make a facade for steam functionality to abstract implementation.
	/// <summary>
	/// It is impossible to be in-game and not connected to the platform (Steam, currently).
	/// The game will exit if Steam is not running, there is no need to check connectivity anywhere.
	/// </summary>
	public class PlatformMediator : MonoBehaviour
	{
		public SteamId Id => SteamClient.SteamId;

		public string Name => SteamClient.Name;

		public IEnumerable<Friend> Friends => SteamFriends.GetFriends();

		public IEnumerable<Friend> Blocked => SteamFriends.GetBlocked();

		public IEnumerable<Friend> Recent => SteamFriends.GetPlayedWith();

		public Image GetAvatar() => GetAvatar(() => SteamFriends.GetLargeAvatarAsync(Id).Result);

		public Image GetAvatar(SteamId steamId) => GetAvatar(() => SteamFriends.GetLargeAvatarAsync(steamId).Result);

		public Image GetAvatar(Friend friend) => GetAvatar(() => friend.GetLargeAvatarAsync().Result);

		//public Lobby? Lobby { get; private set; }

		/*public Lobby? CreateLobby()
		{
			Task<Lobby?> lobbyTask = SteamMatchmaking.CreateLobbyAsync(4);
			lobbyTask.Wait();

			if (lobbyTask.Result.HasValue)
			{
				Lobby = lobbyTask.Result.Value;
				Lobby.Value.SetPrivate();
				Lobby.Value.SetJoinable(true);
			}
			else
			{
				Lobby = null;
			}

			return Lobby;
		}*/

		private void OnEnable()
		{
			SteamUtils.OnSteamShutdown += OnSteamShutdown;
		}

		private void OnDisable()
		{
			SteamUtils.OnSteamShutdown -= OnSteamShutdown;
		}

		private void OnSteamShutdown()
		{
			/*if (Lobby.HasValue)
			{
				Lobby.Value.Leave();
			}*/
		}

		private Image GetAvatar(Func<Image?> predicate)
		{
			Image? avatar = predicate.Invoke();

			if (avatar.HasValue)
			{
				return avatar.Value;
			}

			// Todo: Default image for no avatar instead of just a single black pixel.
			return new Image()
			{
				Data = new byte[1] { 0 },
				Width = 1,
				Height = 1
			};
		}

		#region Ugly Monostate Adapter
		// Todo: Handle platform disconnects in some way.

		/// <summary>
		/// This is just used to know when SteamClient.Shutdown needs to be called duing app exit.
		/// </summary>
		private static bool isConnected = false;

#pragma warning disable IDE0051 // Remove unused private members
		[RuntimeInitializeOnLoadMethod]
		private static void Connect()
		{
			try
			{
				// Todo: Move the app id somewhere else...
				SteamClient.Init(1360690);
				isConnected = true;

#if UNITY_EDITOR
				EditorApplication.playModeStateChanged += OnEditorStop;
#else
				Application.quitting += OnQuit;
#endif
			}
			catch (Exception)
			{
				Debug.LogError("Could not connect to steam, exiting application.");

#if UNITY_EDITOR
				EditorApplication.isPlaying = false;
#else
				// Todo: Show a dialogue if this happens to an end-user.
				Application.Quit(1);
#endif
			}
		}
#pragma warning restore IDE0051

#if UNITY_EDITOR
		private static void OnEditorStop(PlayModeStateChange stateChange)
		{
			if (stateChange == PlayModeStateChange.ExitingPlayMode && isConnected)
			{
				SteamClient.Shutdown();
				EditorApplication.playModeStateChanged -= OnEditorStop;
			}
		}
#else
		private static void OnQuit()
		{
			if (isConnected)
			{
				SteamClient.Shutdown();
				Application.quitting -= OnQuit;
			}
		}
#endif

		#endregion
	}
}