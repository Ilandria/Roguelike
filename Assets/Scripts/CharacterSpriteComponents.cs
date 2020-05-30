using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CCB.Roguelike
{
	[CreateAssetMenu(fileName = "New Character Sprite Components", menuName = "CCB/Singletons/Character Sprite Components")]
	public class CharacterSpriteComponents : ScriptableObject, ILoadable
	{
		[SerializeField]
		private string streamingAssets = Application.streamingAssetsPath;

		[SerializeField]
		private SpriteSheetDescription sheetDescription = null;
		public SpriteSheetDescription SheetDescription => sheetDescription;

		[SerializeField]
		private Texture2D errorTexture = null;

		private Dictionary<CharacterSpriteComponentType, Dictionary<string, CharacterSpriteComponent>> componentsMap = null;

		public IDictionary<string, CharacterSpriteComponent> GetSpriteComponentSheets(CharacterSpriteComponentType type)
		{
			if (componentsMap.ContainsKey(type))
			{
				return componentsMap[type];
			}

			return componentsMap[CharacterSpriteComponentType.Error];
		}

		public CharacterSpriteComponent GetSpriteComponent(CharacterSpriteComponentType type, string name)
		{
			if (componentsMap.ContainsKey(type) && componentsMap[type].ContainsKey(name))
			{
				return componentsMap[type][name];
			}

			return componentsMap[CharacterSpriteComponentType.Error]["Error"];
		}

		public IEnumerator Load(Action<float, string> progress)
		{
			float completedCount = 0.0f;
			componentsMap = new Dictionary<CharacterSpriteComponentType, Dictionary<string, CharacterSpriteComponent>>();
			progress?.Invoke(0.0f, "Character sprites...");
			yield return null;

			// Initialize the error lookup value to make sure this always exists.
			Dictionary<string, CharacterSpriteComponent> errorDict = new Dictionary<string, CharacterSpriteComponent>
			{
				{ "Error", new CharacterSpriteComponent(CharacterSpriteComponentType.Error, "Error", errorTexture) }
			};

			componentsMap.Add(CharacterSpriteComponentType.Error, errorDict);

			// Try to get the json strings defining all of the various character sprite sheets.
			string[] jsonFilePaths = Directory.GetFiles(streamingAssets, "CharacterSpriteCollection.json", SearchOption.AllDirectories);
			int numCollections = jsonFilePaths.Length;

			foreach (string jsonFilePath in jsonFilePaths)
			{
				if (SpriteSheetCollection.TryDeserializeFile(out SpriteSheetCollection collection, jsonFilePath))
				{
					Dictionary<string, CharacterSpriteComponent> components = new Dictionary<string, CharacterSpriteComponent>();
					float sheetLoadCount = 0.0f;

					foreach (SpriteSheetInfo sheet in collection.SpriteSheets)
					{
						// This looks a bit ugly but it's just some math to return detailed loading percentage.
						float outerStep = completedCount / numCollections;
						float innerStep = (sheetLoadCount++ / collection.SpriteSheets.Length) / numCollections;
						progress?.Invoke(outerStep + innerStep, $"{collection.Type} - {sheet.Name}");
						yield return null;

						if (sheet.TryLoadImage(new FileInfo(jsonFilePath).DirectoryName, sheetDescription.SpriteSheetSize))
						{
							components.Add(sheet.Name, new CharacterSpriteComponent(collection.Type, sheet.Name, sheet.SpriteSheet));
						}
					}

					componentsMap.Add(collection.Type, components);
				}

				completedCount++;
				yield return null;
			}

			progress?.Invoke(1.0f, "Character sprites loaded!");
		}

		[Serializable]
		private class SpriteSheetCollection
		{
			[JsonConverter(typeof(StringEnumConverter))]
			public CharacterSpriteComponentType Type { get; set; }
			public SpriteSheetInfo[] SpriteSheets { get; set; }
			public string FileDirectory { get; set; }

			public static bool TryDeserializeFile(out SpriteSheetCollection result, string jsonFilePath)
			{
				try
				{
					result = JsonConvert.DeserializeObject<SpriteSheetCollection>(File.ReadAllText(jsonFilePath), new StringEnumConverter());
					result.FileDirectory = new FileInfo(jsonFilePath).DirectoryName;
					return true;
				}
				catch (Exception e)
				{
					Debug.LogWarning(e);
					result = null;
					return false;
				}
			}
		}

		[Serializable]
		private class SpriteSheetInfo
		{
			public string Name { get; set; }
			public string FileName { get; set; }
			public Texture2D SpriteSheet { get; private set; }

			public bool TryLoadImage(string imageDirectory, Vector2Int spriteSheetSize)
			{
				try
				{
					SpriteSheet = new Texture2D(spriteSheetSize.x, spriteSheetSize.y, TextureFormat.RGBA32, false, false)
					{
						anisoLevel = 1,
						filterMode = FilterMode.Point
					};

					SpriteSheet.LoadImage(File.ReadAllBytes($"{imageDirectory}/{FileName}"), true); // Todo: This may need to be false.
				}
				catch (Exception e)
				{
					Debug.LogWarning(e);
					SpriteSheet = null;
					return false;
				}

				return true;
			}
		}
	}
}