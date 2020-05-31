using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CCB.Roguelike
{
	public class SceneSwitcher : MonoBehaviour
	{
		[SerializeField]
		private string targetSceneName = string.Empty;

		[SerializeField]
		private bool autoActivateOnLoad = false;

		[SerializeField]
		private bool loadComplete = false;

		[SerializeField]
		private UnityEvent onLoadComplete = null;

		private AsyncOperation loadSceneOperation = null;

		public void LoadSceneAsync()
		{
			if (loadSceneOperation == null)
			{
				loadComplete = false;
				AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(targetSceneName);
				loadSceneOperation.completed += OnLoadComplete;
			}
		}

		public void SwitchScene()
		{
			if (loadComplete && loadSceneOperation != null)
			{
				loadComplete = false;
				loadSceneOperation = null;
				SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetSceneName));
			}
		}

		private void OnLoadComplete(AsyncOperation loadSceneOperation)
		{
			loadComplete = true;
			onLoadComplete?.Invoke();

			if (autoActivateOnLoad)
			{
				SwitchScene();
			}
		}
	}
}