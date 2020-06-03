using System.Collections.Generic;
using UnityEngine;

namespace CCB.Roguelike
{
	public class DebugTextRenderer : MonoBehaviour
	{
		[SerializeField]
		private Vector2 screenEdgeOffset = new Vector2(5.0f, 0.0f);

		[SerializeField]
		private Vector2 elementSize = new Vector2(400.0f, 20.0f);

		[SerializeField]
		private float spacing = 0.0f;

		[SerializeField]
		private List<DebugText> debugTexts = new List<DebugText>();

		private Rect drawLocation = Rect.zero;

		public void Add(DebugText debugText)
		{
			if (debugText != null)
			{
				debugTexts.Add(debugText);
			}
		}

		private void OnGUI()
		{
			for (int i = 0; i < debugTexts.Count; i++)
			{
				DebugText debugText = debugTexts[i];
				drawLocation.Set(screenEdgeOffset.x, screenEdgeOffset.y + i * (elementSize.y + spacing), elementSize.x, elementSize.y);
				GUI.color = debugText.Colour;
				GUI.Label(drawLocation, debugText.ToString());
			}
		}
	}
}