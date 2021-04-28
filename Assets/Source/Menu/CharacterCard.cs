using System;
using TMPro;
using UnityEngine;

namespace CCB.Roguelike
{
	public class CharacterCard : MonoBehaviour
	{
		public CharacterDataModel CharacterData { get; set; } = null;

		[SerializeField]
		private CharacterManagementController characterManagementController = null;

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

		[SerializeField]
		private GameObject existingCharacterUi = null;

		[SerializeField]
		private GameObject deleteCharacterUi = null;

		public bool IsCreateCharacterCard { get; private set; } = false;

		public void Start()
		{
			if (CharacterData != null)
			{
				nameText.text = CharacterData.Name;
			}
			else
			{
				IsCreateCharacterCard = true;
				deleteButton.SetActive(false);
				playButton.SetActive(false);
				joinButton.SetActive(false);
				newButton.SetActive(true);
			}

			// Todo: Show some kind of UI warning on characters that are not using the current game's format.
		}

		// Todo: Start game, show character info, etc.

		// Called by the UI to request that this character be deleted.
		public void RequestDelete()
		{
			characterManagementController.RequestCharacterDeletion(CharacterData.Guid);
		}

		// Called by the Character List Controller after RequestDelete is called from the UI button.
		public void ShowDeleteConfirmation()
		{
			existingCharacterUi.SetActive(false);
			deleteCharacterUi.SetActive(true);
		}

		// Called by the UI to confirm character deletion.
		public void HideDeleteConfirmation()
		{
			deleteCharacterUi.SetActive(false);
			existingCharacterUi.SetActive(true);
		}

		public void ConfirmDelete()
		{
			// Notify the management controller of the deletion so it can propagate as required.
			characterManagementController.DeleteCharacter(CharacterData.Guid);
		}

		// Sets the character this card represents as the active character for gameplay and launches the game.
		public void StartSinglePlayer()
		{
			characterManagementController.StartSinglePlayer(CharacterData);
		}
	}
}
