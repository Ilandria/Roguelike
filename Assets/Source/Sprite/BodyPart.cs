using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CCB.Roguelike
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum BodyPart
	{
		Body,
		Ears,
		Eyes,
		Nose,
		Hair,
		Error
	}
}