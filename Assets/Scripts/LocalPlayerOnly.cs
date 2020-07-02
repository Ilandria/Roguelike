using Photon.Pun;

namespace CCB.Roguelike
{
	public class LocalPlayerOnly : MonoBehaviourPun
	{
		private void Start()
		{
			if (!photonView.IsMine)
			{
				Destroy(gameObject);
			}
		}
	}
}