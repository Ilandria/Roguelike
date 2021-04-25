using Newtonsoft.Json;
using System;
using UnityEngine;

namespace CCB.Roguelike
{
	[Serializable]
	public class CharacterDataModel : ICharacterSummary
	{
		[JsonProperty]
		public string DataVersion { get; private set; }

		[JsonProperty]
		public Guid Guid { get; private set; }

		[JsonProperty]
		public string Name { get; private set; } = string.Empty;

		[JsonProperty]
		public int Level { get; private set; } = 1;

		[JsonProperty]
		public DateTime LastPlayed { get; private set; }

		[JsonIgnore]
		public bool IsValid { get; private set; }

		// Todo: More character stats.

		public CharacterDataModel()
		{
			Guid = Guid.NewGuid();
		}

		public void PopulateFromJson(string jsonString)
		{
			try
			{
				JsonConvert.PopulateObject(jsonString, this, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error });
			}
			catch (JsonSerializationException exception)
			{
				IsValid = false;
				Debug.LogWarning($"Issue when loading character ({jsonString}).\n{exception}");
			}
		}

		public string SerializeToJson()
		{
			DataVersion = Application.version;
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}