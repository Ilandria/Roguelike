using System.Collections;
using System.Linq;
using UnityEngine;

namespace CCB.Roguelike
{
	public class CharacterListController : MonoBehaviour
	{
		[SerializeField]
		private CharacterDataRepository characterDataRepository = null;

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
			characterDataRepository.OnNewCharacterCreated += OnNewCharacterCreated;
		}

		private void OnDisable()
		{
			characterDataRepository.OnNewCharacterCreated -= OnNewCharacterCreated;
		}

		private void DestroyAllCards()
		{
			foreach (Transform child in characterList)
			{
				Destroy(child);
			}
		}

		private IEnumerator RegenerateCards()
		{
			DestroyAllCards();

			yield return null;

			// Spawn the card game objects as children of the list UI element.
			foreach (CharacterDataModel dataModel in characterDataRepository.DataModels.OrderByDescending(character => character.LastPlayed))
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

		private void OnNewCharacterCreated(CharacterDataModel newCharacterData)
		{
			createNewCharacterCard.SetParent(null, true);
			CreateCard(newCharacterData);
			createNewCharacterCard.SetParent(characterList, true);
		}
	}
}
