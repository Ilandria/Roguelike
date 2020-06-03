using UnityEngine;
using UnityEngine.SceneManagement;

namespace CCB.Roguelike
{
	public class SceneSwitcher : MonoBehaviour
	{
		[SerializeField]
		private string targetSceneName = string.Empty;

		public void SwitchScene()
		{
			SceneManager.LoadSceneAsync(targetSceneName);
		}
	}
}