using System;

namespace CCB.Roguelike
{
	public class CharacterDataModel : ICharacterSummary
	{
		public Guid Guid { get; private set; }

		public string Name { get; private set; } = string.Empty;

		public int Level { get; private set; }

		public DateTime LastPlayed { get; private set; }

		// Todo: More character stats.

		public void LoadCharacter()
		{
			// Todo: Load from disk, etc.
			Guid = Guid.NewGuid();
			Name = Guid.ToString();
			Level = UnityEngine.Random.Range(1, 101);
			LastPlayed = DateTime.Now;
		}
	}
}