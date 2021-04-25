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

		public bool IsCreateCharacterCard { get; private set; } = false;

		public void Start()
		{
			if (CharacterSummary != null)
			{
				IsCreateCharacterCard = false;
				newButton.SetActive(false);
				nameText.text = CharacterSummary.Name;
			}
			else
			{
				IsCreateCharacterCard = true;
				nameText.text = "Create New";
				deleteButton.SetActive(false);
				playButton.SetActive(false);
				joinButton.SetActive(false);
			}
		}

		// Todo: Start game, delete, show character info, etc.
	}
}
