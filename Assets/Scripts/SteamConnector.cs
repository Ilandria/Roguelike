using UnityEngine;
using Steamworks;
using System;

namespace CCB.Roguelike
{
// Todo: Make a wrapper around steam functionality to abstract implementation.
	public class SteamConnector : MonoBehaviour
	{
		private void Awake()
		{
			try
			{
				SteamClient.Init(1360690);
			}
			catch (Exception)
			{
				Debug.LogError("Could not connect to steam, exiting application.");
				Application.Quit(1);
			}
		}

		private void OnApplicationQuit()
		{
			if (SteamClient.IsValid)
			{
				SteamClient.Shutdown();
			}
		}
	}
}