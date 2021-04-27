using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace CCB.Roguelike
{
	[CreateAssetMenu(fileName = "New Character Data Repository", menuName = "CCB/Data/Character Data Repository")]
	public class CharacterDataRepository : ScriptableObject, ILoadable
	{
		[SerializeField]
		private List<CharacterDataModel> dataModels;

		public IEnumerable<CharacterDataModel> DataModels => dataModels;

		public bool IsLoaded { get; private set; } = false;

		private string savedGamePath = string.Empty;

		public IEnumerator Load(Action<float, string> progress)
		{
			if (!IsLoaded)
			{
				progress(0.0f, "Saved games...");

				savedGamePath = $"{Application.persistentDataPath}/Saved Games";
				Directory.CreateDirectory(savedGamePath);
				string[] characterFiles = Directory.GetFiles(savedGamePath, "*.json", SearchOption.TopDirectoryOnly);
				dataModels = new List<CharacterDataModel>();

				// Load all characters.
				for (int i = 0; i < characterFiles.Length; i++)
				{
					string fileName = characterFiles[i];

					// Todo: Might want to catch file reading exceptions.
					CharacterDataModel dataModel = CharacterDataModel.CreateFromJson(File.ReadAllText(fileName, System.Text.Encoding.UTF8));

					// Only load characters with valid guid file names that match the loaded character.
					if (Path.GetFileNameWithoutExtension(fileName).Equals(dataModel?.Guid.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						dataModels.Add(dataModel);
					}
					else
					{
						Debug.LogWarning($"Failed to load character ({fileName}). Character id does not match filename or is an invalid format.");
					}

					progress((i + 1) / (float)(characterFiles.Length + 1), $"Character {dataModel.Name}");

					yield return new WaitForSeconds(0.1f);
				}

				IsLoaded = true;
			}

			progress(1.0f, "Character list loaded!");
		}

		private void OnEnable()
		{
			Unload();
		}

		public void Unload()
		{
			IsLoaded = false;
			dataModels = null;
		}

		public bool Contains(Guid characterGuid)
		{
			bool contains = false;

			if (IsLoaded)
			{
				contains = dataModels.Find(data => data.Guid.Equals(characterGuid)) != null;
			}

			return contains;
		}

		/// <summary>
		/// Add the given character data to the repository without saving it to disk.
		/// </summary>
		/// <param name="characterData">The character data to add.</param>
		/// <returns>True if successful, false otherwise (duplicate entry, etc.).</returns>
		public bool AddCharacterData(CharacterDataModel characterData)
		{
			bool success = false;

			if (IsLoaded && !Contains(characterData.Guid))
			{
				dataModels.Add(characterData);
				success = true;
			}

			return success;
		}

		/// <summary>
		/// Save the given character to disk in the most recent data format.
		/// </summary>
		/// <param name="characterGuid">Guid of the character data to save to disk.</param>
		/// <returns>True if success, false otherwise.</returns>
		public bool SaveCharacter(Guid characterGuid)
		{
			bool success = false;

			if (IsLoaded)
			{
				CharacterDataModel characterToSave = dataModels.Find(data => data.Guid.Equals(characterGuid));

				if (characterToSave != null)
				{
					string targetFilePath = $"{savedGamePath}/{characterGuid}";
					string tempFile = $"savedata.tmp";

					try
					{
						// Get the serialized json string defining the character. Save data version is updated to current format.
						// Write to a temp file then copy to the original in case something goes wrong during the write.
						File.WriteAllText(tempFile, characterToSave.SerializeToJson(), System.Text.Encoding.UTF8);
						File.Copy(tempFile, $"{targetFilePath}.json", true);
						success = true;
					}
					catch (Exception exception)
					{
						Debug.LogWarning($"Failed to save character to file ({targetFilePath}).\n{exception}");
						// Todo: Create a new file with the failed save for the player to reference?
					}

					File.Delete(tempFile);
				}
			}

			return success;
		}

		/// <summary>
		/// Save the given character to disk in the most recent data format.
		/// </summary>
		/// <param name="characterData">Character data to save to disk.</param>
		/// <returns>True if success, false otherwise.</returns>
		public bool SaveCharacter(CharacterDataModel characterData)
		{
			return SaveCharacter(characterData.Guid);
		}

		/// <summary>
		/// Saves all loaded characters to disk in the most recent data format.
		/// </summary>
		/// <returns>A collection containing the success/failure states of saving each character.</returns>
		public IEnumerable<KeyValuePair<CharacterDataModel, bool>> SaveAllCharacters()
		{
			Dictionary<CharacterDataModel, bool> results = new Dictionary<CharacterDataModel, bool>();

			if (IsLoaded)
			{
				foreach (CharacterDataModel data in DataModels)
				{
					results.Add(data, SaveCharacter(data.Guid));
				}
			}

			return results;
		}

		/// <summary>
		/// Removes the character with the given guid from the repository and deletes its data from disk.
		/// </summary>
		/// <returns>True if successfully removed and deleted, false otherwise.</returns>
		public bool DeleteCharacter(Guid characterGuid)
		{
			bool success = false;

			if (IsLoaded)
			{
				CharacterDataModel characterToDelete = DataModels.DefaultIfEmpty(null).SingleOrDefault(data => data.Guid.Equals(characterGuid));

				if (characterToDelete != null)
				{
					dataModels.Remove(characterToDelete);
					File.Delete($"{savedGamePath}/{characterGuid}.json");
					success = true;
				}
			}

			return success;
		}

		/// <summary>
		/// Creates a new, empty character then saves it to disk.
		/// </summary>
		/// <returns>The <see cref="CharacterDataModel"/> that was created if successfully created and saved, null otherwise.</returns>
		public CharacterDataModel CreateNewCharacter()
		{
			CharacterDataModel newCharacter = new CharacterDataModel();

			// Attempt to add the new character.
			if (AddCharacterData(newCharacter))
			{
				// Attempt to save the new character if it was added.
				if(!SaveCharacter(newCharacter))
				{
					// Remove it if it failed to save.
					dataModels.Remove(newCharacter);
					newCharacter = null;
				}
			}

			return newCharacter;
		}
	}
}