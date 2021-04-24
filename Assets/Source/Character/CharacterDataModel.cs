using Newtonsoft.Json;
using System;

namespace CCB.Roguelike
{
	[Serializable]
	public class CharacterDataModel : ICharacterSummary
	{
		[JsonProperty]
		public int DataVersion { get; private set; }

		[JsonProperty]
		public Guid Guid { get; private set; }

		[JsonProperty]
		public string Name { get; private set; } = string.Empty;

		[JsonProperty]
		public int Level { get; private set; }

		[JsonProperty]
		public DateTime LastPlayed { get; private set; }

		// Todo: More character stats.

		public void PopulateFromJson(string jsonString)
		{
			// Todo: This should probably be a bit more robust.
			JsonConvert.PopulateObject(jsonString, this, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error });
		}

		public string SerializeToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}