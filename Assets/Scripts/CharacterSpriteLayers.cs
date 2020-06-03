using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace CCB.Roguelike
{
	[CreateAssetMenu(fileName = "New Character Sprite Components", menuName = "CCB/Singleton/Character Sprite Components")]
	public class CharacterSpriteLayers : ScriptableObject, ILoadable
	{
		[SerializeField]
		private string streamingAssets = Application.streamingAssetsPath;

		[SerializeField]
		private SpriteSheetDescription sheetDescription = null;
		public SpriteSheetDescription SheetDescription => sheetDescription;

		[SerializeField]
		private Texture2D errorTexture = null;

		private IEnumerable<CharacterSpriteLayer> components = null;
		private CharacterSpriteLayer errorSprite = null;

		public bool IsLoaded { get; private set; } = false;

		public CharacterSpriteLayer GetLayer(CharacterBodyType body, CharacterPartType part, string name) =>
			components.Where(component => component.Body == body && component.Part == part && component.Name == name).DefaultIfEmpty(errorSprite).SingleOrDefault();

		public IEnumerable<CharacterSpriteLayer> GetSpriteSheets(CharacterBodyType body, CharacterPartType part) =>
			components.Where(component => component.Body == body && component.Part == part).DefaultIfEmpty(errorSprite);

		public IEnumerator Load(Action<float, string> progress)
		{
			if (errorSprite == null)
			{
				float completedCount = 0.0f;
				progress?.Invoke(0.0f, "Character sprites...");

				string[] jsonFilePaths = Directory.GetFiles(streamingAssets, "CharacterSpriteCollection.json", SearchOption.AllDirectories);
				int numCollections = jsonFilePaths.Length;
				errorSprite = new CharacterSpriteLayer(CharacterBodyType.Error, CharacterPartType.Error, "Error", errorTexture);
				HashSet<CharacterSpriteLayer> componentsSet = new HashSet<CharacterSpriteLayer>();
				yield return null;

				foreach (string jsonFilePath in jsonFilePaths)
				{
					if (SpriteSheetCollection.TryDeserializeFile(out SpriteSheetCollection collection, jsonFilePath))
					{
						float sheetLoadCount = 0.0f;

						foreach (SpriteSheetInfo sheet in collection.SpriteSheets)
						{
							// This looks a bit ugly but it's just some math to return detailed loading percentage.
							float outerStep = completedCount / numCollections;
							float innerStep = sheetLoadCount++ / collection.SpriteSheets.Length / numCollections;
							progress?.Invoke(outerStep + innerStep, $"{collection.Body} - {collection.Part} - {sheet.Name}");
							yield return null;

							if (sheet.TryLoadImage(new FileInfo(jsonFilePath).DirectoryName, sheetDescription.SpriteSheetSize))
							{
								componentsSet.Add(new CharacterSpriteLayer(collection.Body, collection.Part, sheet.Name, sheet.SpriteSheet));
							}
						}
					}

					completedCount++;
					yield return null;
				}

				components = componentsSet;
			}

			progress?.Invoke(1.0f, "Character sprites loaded!");
		}

		private void OnEnable()
		{
			errorSprite = null;
			components = null;
		}

		[Serializable]
		private class SpriteSheetCollection
		{
			[JsonConverter(typeof(StringEnumConverter))]
			public CharacterBodyType Body { get; set; }

			[JsonConverter(typeof(StringEnumConverter))]
			public CharacterPartType Part { get; set; }

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