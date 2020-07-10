using UnityEngine;

namespace CCB.Roguelike
{
	public class PercentFormatter : MonoBehaviour
	{
		[SerializeField]
		private UnityEventString onPercentChanged = null;

		public void FormatPercent(float percent)
		{
			onPercentChanged?.Invoke($"{Mathf.RoundToInt(percent * 100.0f)}%");
		}
	}
}