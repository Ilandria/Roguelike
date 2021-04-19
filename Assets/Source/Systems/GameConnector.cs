using UnityEngine;

namespace CCB.Roguelike
{
	public class GameConnector : MonoBehaviour
	{
		[SerializeField]
		private NetworkMediator networkMediator = null;

		public void ConnectToGame()
		{
			networkMediator.CreateGame();
			// Todo: Join game, reconnect to game, etc. May need to create a game state object.
		}
	}
}