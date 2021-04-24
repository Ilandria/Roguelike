using TMPro;
using UnityEngine;

namespace CCB.Roguelike
{
	public class CharacterCard : MonoBehaviour
	{
		public ICharacterSummary CharacterSummary { get; set; } = null;

		[SerializeField]
		private TextMeshProUGUI nameText = null;

		[SerializeField]
		private GameObject newButton = null;

		[SerializeField]
		private GameObject deleteButton = null;

		[SerializeField]
		private GameObject playButton = null;

		[SerializeField]
		private GameObject joinButton = null;

		public void Start()
		{
			if (CharacterSummary != null)
			{
				newButton.SetActive(false);
				nameText.text = CharacterSummary.Name;
			}
			else
			{
				nameText.text = "New Character";
				deleteButton.SetActive(false);
				playButton.SetActive(false);
				joinButton.SetActive(false);
			}
		}

		// Todo: Start game, delete, show character info, etc.
	}
}
