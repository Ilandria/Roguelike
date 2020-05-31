﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace CCB.Roguelike
{
	public class LoadableLoader : MonoBehaviour, ILoadable
	{
		private readonly DebugText progressDebugText = new DebugText { Name = "Load Progress", Unit = "%" };
		private readonly DebugText statusDebugText = new DebugText { Name = "Load Status" };

		[SerializeField]
		private DebugTextRenderer debugTextRenderer = null;

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
		private UnityEvent onLoadProgress = null;

		[SerializeField]
		private UnityEvent onLoadComplete = null;

		private Coroutine loadingCoroutine = null;

		public void StartLoading()
		{
			if (loadingCoroutine == null)
			{
				loadingCoroutine = StartCoroutine(Load(
					(progress, status) =>
					{
						loadProgress = progress;
						loadStatus = status;

						if (debugTextRenderer != null)
						{
							progressDebugText.Value = string.Format("{0:0.0}", loadProgress * 100.0f);
							statusDebugText.Value = status;
						}

						onLoadProgress?.Invoke();
					}));
			}
		}

		public void CancelLoading()
		{
			if (loadingCoroutine != null)
			{
				StopCoroutine(loadingCoroutine);
			}
		}

		public IEnumerator Load(Action<float, string> progress)
		{
			onLoadStart?.Invoke();
			float loadedLoadables = 0.0f;

			foreach (ILoadable loadable in loadables)
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
			foreach (Object loadable in loadables)
			{
				if (loadable as ILoadable == null)
				{
					loadables.Remove(loadable);
					Debug.LogWarning($"Removed {loadable.name} from LoadableLoader {name}'s loadables list as it is not an ILoadable.");
				}
			}

			if (loadOnAwake)
			{
				StartLoading();
			}

			debugTextRenderer?.Add(progressDebugText);
			debugTextRenderer?.Add(statusDebugText);
		}
	}
}