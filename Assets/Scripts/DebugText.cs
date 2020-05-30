using System;
using UnityEngine;

namespace CCB.Roguelike
{
	[Serializable]
    public class DebugText
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public Color Colour { get; set; } = Color.white;

		public override string ToString()
		{
            return string.Format("{0}:{1,35}{2}", Name, Value, Unit);
        }
	}
}