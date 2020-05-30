using System.Collections.Generic;
using UnityEngine;

namespace CCB.Roguelike
{
	public class DebugTextRenderer : MonoBehaviour
	{
		[SerializeField]
		private Vector2 screenEdgeOffset = Vector2.zero;

		[SerializeField]
		private Vector2 elementSize = Vector2.zero;

		[SerializeField]
		private float spacing = 0.0f;

		[SerializeField]
		private List<DebugText> debugTexts = null;

		private Rect drawLocation = Rect.zero;

		private void Awake()
		{
			debugTexts = new List<DebugText>();
		}

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