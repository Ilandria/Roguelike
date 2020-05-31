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

		private IEnumerable<CharacterSpriteComponent> components = null;
		private CharacterSpriteComponent errorSprite = null;

		public CharacterSpriteComponent GetSpriteSheet(Func<CharacterSpriteComponent, bool> query) =>
			components.DefaultIfEmpty(errorSprite).SingleOrDefault(component => query(component));

		public CharacterSpriteComponent GetSpriteSheet(CharacterBodyType body, CharacterPartType part, string name) =>
			components.DefaultIfEmpty(errorSprite).SingleOrDefault(component => component.Body == body && component.Part == part && component.Name == name);

		public IEnumerable<CharacterSpriteComponent> GetSpriteSheets(Func<CharacterSpriteComponent, bool> query) =>
			components.DefaultIfEmpty(errorSprite).Where(component => query(component));

		public IEnumerable<CharacterSpriteComponent> GetSpriteSheets(CharacterBodyType body, CharacterPartType part) =>
			components.DefaultIfEmpty(errorSprite).Where(component => component.Body == body && component.Part == part);

		public IEnumerable<CharacterSpriteComponent> GetSpriteSheets(CharacterBodyType body) =>
			components.DefaultIfEmpty(errorSprite).Where(component => component.Body == body);

		public IEnumerator Load(Action<float, string> progress)
		{
			float completedCount = 0.0f;
			progress?.Invoke(0.0f, "Character sprites...");
			yield return null;

			string[] jsonFilePaths = Directory.GetFiles(streamingAssets, "CharacterSpriteCollection.json", SearchOption.AllDirectories);
			int numCollections = jsonFilePaths.Length;
			errorSprite = new CharacterSpriteComponent(CharacterBodyType.Error, CharacterPartType.Error, "Error", errorTexture);
			HashSet<CharacterSpriteComponent> componentsSet = new HashSet<CharacterSpriteComponent>();

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
							componentsSet.Add(new CharacterSpriteComponent(collection.Body, collection.Part, sheet.Name, sheet.SpriteSheet));
						}
					}
				}

				completedCount++;
				yield return null;
			}

			components = componentsSet;
			progress?.Invoke(1.0f, "Character sprites loaded!");
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