using CCB.Roguelike;
using UnityEngine;

[CreateAssetMenu(fileName = "New Network Connection Info", menuName = "CCB/Data/Network Connection Info")]
public class NetworkConnectionInfo : ScriptableObject
{
	public NetworkConnectionState connectionState = NetworkConnectionState.DisconnectedFromMaster;

	public string roomName = string.Empty;
}
