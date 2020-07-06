using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CCB.Roguelike
{
	// Todo: Make a facade for steam functionality to abstract implementation.
	[CreateAssetMenu(fileName = "New Platform Connector", menuName = "CCB/Data/Platform Connector")]
	public class PlatformConnector : ScriptableObject
	{
		public bool IsConnected
		{
			get => isConnected;
			private set => isConnected = value;
		}

		public SteamId SteamId => IsConnected ? SteamClient.SteamId : new SteamId();

		public IEnumerable<Friend> Friends => IsConnected ? SteamFriends.GetFriends() : new List<Friend>();

		public IEnumerable<Friend> Blocked => IsConnected ? SteamFriends.GetBlocked() : new List<Friend>();

		public IEnumerable<Friend> Recent => IsConnected ? SteamFriends.GetPlayedWith() : new List<Friend>();

		public Image GetAvatar() => GetAvatar(() => SteamFriends.GetLargeAvatarAsync(SteamId).Result);

		public Image GetAvatar(SteamId steamId) => GetAvatar(() => SteamFriends.GetLargeAvatarAsync(steamId).Result);

		public Image GetAvatar(Friend friend) => GetAvatar(() => friend.GetLargeAvatarAsync().Result);

		private Image GetAvatar(Func<Image?> predicate)
		{
			if (IsConnected)
			{
				Image? avatar = predicate.Invoke();

				if (avatar.HasValue)
				{
					return avatar.Value;
				}
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

		private static bool isConnected = false;

#pragma warning disable IDE0051 // Remove unused private members
		[RuntimeInitializeOnLoadMethod]
		private static void Connect()
		{
			if (!isConnected)
			{
				try
				{
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
					Application.Quit(1);
#endif
				}
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