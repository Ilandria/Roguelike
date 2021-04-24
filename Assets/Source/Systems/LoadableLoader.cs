using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace CCB.Roguelike
{
	public class LoadableLoader : MonoBehaviour
	{
		// Todo: This needs to be renamed to something like "loadableObjects" for clarity.
		[SerializeField]
		private List<Object> loadables = null;

		[SerializeField]
		private bool loadOnAwake = false;

		[SerializeField]
		private float loadProgress = 0.0f;
		public float LoadProgress => loadProgress;

		[SerializeField]
		private string loadStatus = string.Empty;
		public string LoadStatus => loadStatus;

		[SerializeField]
		private UnityEvent onLoadStart = null;

		[SerializeField]
		private UnityEventFloat onLoadProgress = null;

		[SerializeField]
		private UnityEventString onLoadStatus = null;

		[SerializeField]
		private UnityEvent onLoadComplete = null;

		private Coroutine loadingCoroutine = null;

		// Todo: Rename to "loadables" once the list above is renamed.
		private List<ILoadable> iLoadables = null;

		public void StartLoading()
		{
			if (loadingCoroutine == null)
			{
				loadingCoroutine = StartCoroutine(Load(
					(progress, status) =>
					{
						loadProgress = progress;
						loadStatus = status;
						onLoadProgress?.Invoke(progress);
						onLoadStatus?.Invoke(status);
					}));
			}
		}

		private IEnumerator Load(Action<float, string> progress)
		{
			onLoadStart?.Invoke();
			float loadedLoadables = 0.0f;

			foreach (ILoadable loadable in iLoadables)
			{
				yield return loadable.Load((pct, status) =>
				{
					progress?.Invoke(loadedLoadables / loadables.Count + pct / loadables.Count, status);
				});

				loadedLoadables++;
			}

			progress?.Invoke(1.0f, "Loading complete!");
			onLoadComplete?.Invoke();
			loadingCoroutine = null;
		}

		private void Awake()
		{
			// Todo: Move this out to some options object - don't hardcode it here.
			Application.targetFrameRate = 60;

			iLoadables = new List<ILoadable>();

			foreach (Object loadable in loadables)
			{
				// Needed because the List<Object> assumes added GameObjects are GameObjects since Unity has no support for referencing interfaces.
				if (loadable as GameObject != null)
				{
					// In case there are multiple ILoadables on the object...
					foreach (ILoadable actualLoadable in ((GameObject)loadable).GetComponents<ILoadable>())
					{
						iLoadables.Add(actualLoadable);
					}
				}
				// Scriptable object ILoadables get caught here.
				else if (loadable as ILoadable != null)
				{
					iLoadables.Add(loadable as ILoadable);
				}
			}

			if (loadOnAwake)
			{
				StartLoading();
			}
		}
	}
}