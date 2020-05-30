using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CCB.Roguelike
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum CharacterSpriteComponentType
	{
		FemaleBody,
		FemaleEars,
		FemaleEyes,
		FemaleNose,
		FemaleHair,
		MaleBody,
		MaleEars,
		MaleEyes,
		MaleNose,
		MaleHair,
		Error
	}
}