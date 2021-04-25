using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

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

					CharacterDataModel dataModel = new CharacterDataModel();

					// Todo: Might want to catch file reading exceptions.
					dataModel.PopulateFromJson(File.ReadAllText(fileName, System.Text.Encoding.UTF8));

					// Only load characters with valid guid file names that match the loaded character.
					if (Path.GetFileNameWithoutExtension(fileName).Equals(dataModel.Guid.ToString(), StringComparison.OrdinalIgnoreCase))
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

		public void AddCharacterData(CharacterDataModel characterData)
		{
			if (IsLoaded && !dataModels.Where(data => data.Guid.Equals(characterData.Guid)).Any())
			{
				dataModels.Add(characterData);
			}
		}

		public void SaveCharacter(Guid characterGuid)
		{
			if (IsLoaded)
			{
				string characterJson = DataModels.DefaultIfEmpty(null).SingleOrDefault(data => data.Guid.Equals(characterGuid))?.SerializeToJson();

				if (!string.IsNullOrWhiteSpace(characterJson))
				{
					string targetFilePath = $"{savedGamePath}/{characterGuid}";
					string tempFile = $"savedata.tmp";

					try
					{
						File.WriteAllText(tempFile, characterJson, System.Text.Encoding.UTF8);
						File.Copy(tempFile, $"{targetFilePath}.json", true);
					}
					catch (Exception exception)
					{
						Debug.LogWarning($"Failed to save character to file ({targetFilePath}).\n{exception}");
						// Todo: Create a new file with the failed save for the player to reference?
					}

					File.Delete(tempFile);
				}
			}
		}

		public void SaveAllCharacters()
		{
			if (IsLoaded)
			{
				foreach (CharacterDataModel data in DataModels)
				{
					SaveCharacter(data.Guid);
				}
			}
		}

		public void DeleteCharacter(Guid characterGuid)
		{
			if (IsLoaded)
			{
				CharacterDataModel characterToDelete = DataModels.DefaultIfEmpty(null).SingleOrDefault(data => data.Guid.Equals(characterGuid));

				if (characterToDelete != null)
				{
					dataModels.Remove(characterToDelete);
					File.Delete($"{savedGamePath}/{characterGuid}.json");
				}
			}
		}
	}
}