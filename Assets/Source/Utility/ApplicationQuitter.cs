using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace CCB.Roguelike
{
	public class ApplicationQuitter : MonoBehaviour
	{
		[SerializeField]
		private UnityEvent onQuitApplication = null;

		public void QuitApplication()
		{
#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
#else
			Application.Quit(1);
#endif
			onQuitApplication?.Invoke();
		}
	}
}