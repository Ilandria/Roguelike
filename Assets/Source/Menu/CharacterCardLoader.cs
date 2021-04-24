using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

			IsLoaded = false;
		}

		public IEnumerator Load(Action<float, string> progress)
		{
			progress(0.0f, "Character data...");

			string savedGamePath = $"{Application.persistentDataPath}/Saved Games";
			Directory.CreateDirectory(savedGamePath);
			string[] characterFiles = Directory.GetFiles(savedGamePath, "*.json", SearchOption.TopDirectoryOnly);

			List<CharacterDataModel> loadedCharacters = new List<CharacterDataModel>();

			// Load all characters.
			for (int i = 0; i < characterFiles.Length; i++)
			{
				try
				{
					// Todo: Load data models from disk.
					CharacterDataModel dataModel = new CharacterDataModel();
					dataModel.PopulateFromJson(File.ReadAllText(characterFiles[i]));
					loadedCharacters.Add(dataModel);
					progress((i + 1) / (float)(characterFiles.Length + 1), $"Character {dataModel.Name}");
				}
				catch (JsonSerializationException exception)
				{
					Debug.LogWarning($"Failed to load character ({characterFiles[i]}).\n{exception}");
				}

				yield return new WaitForSeconds(0.1f);
			}

			progress(0.95f, "Populating character list...");

			// Spawn the card game objects as children of the list UI element.
			foreach (CharacterDataModel dataModel in loadedCharacters.OrderByDescending(character => character.LastPlayed))
			{
				// Todo: These need to be tracked in some kind of character registry so other components can access it.
				CharacterCard characterCard = Instantiate(characterCardPrefab, characterList, false).GetComponent<CharacterCard>();
				characterCard.CharacterSummary = dataModel;
				yield return null;
			}

			// The last card is a "create character" card.
			Instantiate(characterCardPrefab, characterList, false);

			IsLoaded = true;
			progress(1.0f, "Character list loaded!");
		}
	}
}
