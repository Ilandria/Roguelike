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
	public class SpriteLayers : ScriptableObject, ILoadable
	{
		[SerializeField]
		private SpriteSheetDescription sheetDescription = null;
		public SpriteSheetDescription SheetDescription => sheetDescription;

		[SerializeField]
		private Texture2D errorTexture = null;

		private IEnumerable<SpriteLayer> components = null;
		private SpriteLayer errorSprite = null;

		public bool IsLoaded { get; private set; } = false;

		public SpriteLayer GetLayer(BodyBase bodyBase, BodyPart part, string name) =>
			components.Where(component => component.BodyBase == bodyBase && component.BodyPart == part && component.Name == name).DefaultIfEmpty(errorSprite).SingleOrDefault();

		public SpriteLayer GetLayer(BodyBase bodyBase, BodyPart part, int id) =>
			components.Where(component => component.BodyBase == bodyBase && component.BodyPart == part && component.Id == id).DefaultIfEmpty(errorSprite).SingleOrDefault();

		public IEnumerator Load(Action<float, string> progress)
		{
			if (errorSprite == null)
			{
				errorSprite = new SpriteLayer(BodyBase.Error, BodyPart.Error, -1, "Error", errorTexture);

				float completedCount = 0.0f;
				progress?.Invoke(0.0f, "Character sprites...");

				string[] jsonFilePaths = Directory.GetFiles(Application.streamingAssetsPath, "CharacterSpriteCollection.json", SearchOption.AllDirectories);
				int numCollections = jsonFilePaths.Length;
				HashSet<SpriteLayer> componentsSet = new HashSet<SpriteLayer>();
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
							progress?.Invoke(outerStep + innerStep, $"{collection.Body} | {collection.Part} | {sheet.Name}");
							yield return null;

							if (sheet.TryLoadImage(new FileInfo(jsonFilePath).DirectoryName, sheetDescription.SpriteSheetSize))
							{
								componentsSet.Add(new SpriteLayer(collection.Body, collection.Part, sheet.Id, sheet.Name, sheet.SpriteSheet));
							}
						}
					}

					completedCount++;
					yield return null;
				}

				components = componentsSet;
			}

			IsLoaded = true;
			progress?.Invoke(1.0f, "Character sprites loaded!");
		}

		private void OnEnable()
		{
			IsLoaded = false;
			errorSprite = null;
			components = null;
		}

		[Serializable]
		private class SpriteSheetCollection
		{
			[JsonConverter(typeof(StringEnumConverter))]
			public BodyBase Body { get; set; }

			[JsonConverter(typeof(StringEnumConverter))]
			public BodyPart Part { get; set; }

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

		/// <summary>
		/// This is what is directly deserialized by Json.Net
		/// </summary>
		[Serializable]
		private class SpriteSheetInfo
		{
			public int Id { get; set; }

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