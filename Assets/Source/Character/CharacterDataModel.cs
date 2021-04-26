using Newtonsoft.Json;
using System;
using UnityEngine;

namespace CCB.Roguelike
{
	[Serializable]
	public class CharacterDataModel : ICharacterSummary
	{
		[JsonProperty]
		public string DataVersion { get; private set; } = string.Empty;

		[JsonProperty]
		public Guid Guid { get; private set; }

		[JsonProperty]
		public string Name { get; private set; } = string.Empty;

		[JsonProperty]
		public int Level { get; private set; } = 1;

		[JsonProperty]
		public DateTime LastPlayed { get; private set; }

		[JsonProperty]
		public bool IsNewCharacter { get; private set; }

		// Todo: More character stats.

		public CharacterDataModel()
		{
			DataVersion = Application.version;
			Guid = Guid.NewGuid();
			Name = "New Character";
			IsNewCharacter = true;
		}

		public static CharacterDataModel CreateFromJson(string jsonString)
		{
			return JsonConvert.DeserializeObject<CharacterDataModel>(jsonString);
		}

		public void PopulateFromJson(string jsonString)
		{
			JsonConvert.PopulateObject(jsonString, this);
		}

		public string SerializeToJson()
		{
			if (!DataVersion.Equals(Application.version))
			{
				Debug.Log($"Migrating character data for {Guid} ({Name}): {DataVersion} => {Application.version}");

				// Todo: Handle any special case for version conversions here. For right now it's just assuming everything's fine.
				DataVersion = Application.version;
			}

			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}