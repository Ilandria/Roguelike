using System;

namespace CCB.Roguelike
{
	public interface ICharacterSummary
	{
		bool IsValid { get; }

		Guid Guid { get; }

		string Name { get; }

		int Level { get; }

		DateTime LastPlayed { get; }
	}
}