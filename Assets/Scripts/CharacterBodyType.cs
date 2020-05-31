using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CCB.Roguelike
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum CharacterBodyType
	{
		Female,
		Male,
		Error
	}
}