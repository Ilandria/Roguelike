using System;

namespace CCB.Roguelike
{
	public interface ICharacterSummary
	{
		Guid Guid { get; }

		string Name { get; }

		int Level { get; }

		DateTime LastPlayed { get; }
	}
}