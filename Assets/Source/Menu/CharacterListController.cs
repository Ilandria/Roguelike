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

		public void CreateCards()
		{
			StartCoroutine(RegenerateCards());
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
				CharacterCard characterCard = Instantiate(characterCardPrefab, characterList, false).GetComponent<CharacterCard>();
				characterCard.CharacterSummary = dataModel;
				yield return null;
			}

			// The last card is a "create character" card.
			Instantiate(characterCardPrefab, characterList, false);
		}
	}
}
