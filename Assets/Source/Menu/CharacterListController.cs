using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace CCB.Roguelike
{
	public class CharacterListController : MonoBehaviour
	{
		[SerializeField]
		private CharacterManagementController characterManagementController = null;

		[SerializeField]
		private GameObject characterCardPrefab = null;

		[SerializeField]
		private Transform characterList = null;

		private Transform createNewCharacterCard = null;

		public void CreateCards()
		{
			StartCoroutine(RegenerateCards());
		}

		private void OnEnable()
		{
			characterManagementController.OnNewCharacterCreated += OnNewCharacterCreated;
			characterManagementController.OnCharacterDeleted += OnCharacterDeleted;
			characterManagementController.OnCharacterDeleteRequested += OnCharacterDeleteRequested;
		}

		private void OnDisable()
		{
			characterManagementController.OnNewCharacterCreated -= OnNewCharacterCreated;
			characterManagementController.OnCharacterDeleted -= OnCharacterDeleted;
			characterManagementController.OnCharacterDeleteRequested -= OnCharacterDeleteRequested;
		}

		private void DestroyAllCards()
		{
			foreach (Transform child in characterList)
			{
				Destroy(child);
			}
		}

		private void DestroyCard(Guid characterGuid)
		{
			foreach (Transform child in characterList)
			{
				CharacterCard card = child.GetComponent<CharacterCard>();

				// CharacterSummary is null on the last child since it's the "create character" card.
				if (card.CharacterSummary != null && card.CharacterSummary.Guid.Equals(characterGuid))
				{
					Destroy(child.gameObject);
				}
			}
		}

		private IEnumerator RegenerateCards()
		{
			DestroyAllCards();

			yield return null;

			// Spawn the card game objects as children of the list UI element.
			foreach (CharacterDataModel dataModel in characterManagementController.Characters.OrderByDescending(character => character.LastPlayed))
			{
				CreateCard(dataModel);
				yield return null;
			}

			// The last card is a "create character" card.
			createNewCharacterCard = Instantiate(characterCardPrefab, characterList, false).transform;
		}

		private void CreateCard(CharacterDataModel characterData)
		{
			CharacterCard characterCard = Instantiate(characterCardPrefab, characterList, false).GetComponent<CharacterCard>();
			characterCard.CharacterSummary = characterData;
		}

		// Raised by the Character Management Controller when a new character is created and saved to disk.
		private void OnNewCharacterCreated(CharacterDataModel newCharacterData)
		{
			createNewCharacterCard.SetParent(null, true);
			CreateCard(newCharacterData);
			createNewCharacterCard.SetParent(characterList, true);
		}

		// Raised by the Character Management Controller when a Character Card requests to be deleted.
		private void OnCharacterDeleteRequested(Guid characterGuid)
		{
			// Todo: Show an "Are you sure you want to delete this character?" prompt before firing this delete event.
			characterManagementController.DeleteCharacter(characterGuid);
		}

		// Raised by the Character Management Controller when a character's data has been unloaded and deleted from disk.
		private void OnCharacterDeleted(Guid characterGuid)
		{
			DestroyCard(characterGuid);
		}
	}
}
