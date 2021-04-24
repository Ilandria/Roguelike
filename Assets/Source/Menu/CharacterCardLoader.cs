using System;
using System.Collections;
using UnityEngine;

namespace CCB.Roguelike
{
	public class CharacterCardLoader : MonoBehaviour, ILoadable
	{
		[SerializeField]
		private GameObject characterCardPrefab = null;

		[SerializeField]
		private Transform characterList = null;

		public bool IsLoaded { get; private set; } = false;

		public void DestroyAllCards()
		{
			foreach (Transform child in characterList)
			{
				Destroy(child);
			}
		}

		public IEnumerator Load(Action<float, string> progress)
		{
			progress(0.0f, "Character list...");

			// Todo: Load all characters and add them to the characterList as a child. This is testing code.
			for (int i = 1; i < 10; i++)
			{
				// Todo: Load data models first, sort them by most-recently played date, then instantiate the cards.
				CharacterCard characterCard = Instantiate(characterCardPrefab, characterList, false).GetComponent<CharacterCard>();
				CharacterDataModel dataModel = new CharacterDataModel();
				dataModel.LoadCharacter();
				characterCard.CharacterSummary = dataModel;

				progress(i / 10.0f, $"Character {dataModel.Guid}");
				yield return new WaitForSeconds(0.25f);
			}

			// The last card is a "create character" card.
			Instantiate(characterCardPrefab, characterList, false);

			IsLoaded = true;
			progress(1.0f, "Character list loaded!");
		}
	}
}
