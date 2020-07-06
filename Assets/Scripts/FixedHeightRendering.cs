using UnityEngine;
using UnityEngine.Rendering;

namespace CCB.Roguelike
{
	[RequireComponent(typeof(Camera))]
	public class FixedHeightRendering : MonoBehaviour
	{
		[SerializeField]
		private Camera sourceCamera = null;

		[SerializeField]
		private int pixelHeight = 720;

		private RenderTexture downscaleTarget;

		private void OnEnable()
		{
			RenderPipelineManager.beginCameraRendering += OnBeginRender;
			RenderPipelineManager.endCameraRendering += OnEndRender;

			float aspectRatio = Screen.width / (float)Screen.height;
			downscaleTarget = new RenderTexture((int)(pixelHeight * aspectRatio), pixelHeight, 0, RenderTextureFormat.ARGB32)
			{
				filterMode = FilterMode.Point
			};
		}

		private void OnDisable()
		{
			RenderPipelineManager.beginCameraRendering -= OnBeginRender;
			RenderPipelineManager.endCameraRendering -= OnEndRender;
			downscaleTarget.Release();
			downscaleTarget = null;
			sourceCamera.targetTexture = null;
		}

		private void OnBeginRender(ScriptableRenderContext context, Camera camera)
		{
			if (camera == sourceCamera)
			{
				sourceCamera.targetTexture = downscaleTarget;
			}
		}

		private void OnEndRender(ScriptableRenderContext context, Camera camera)
		{
			if (camera == sourceCamera)
			{
				sourceCamera.targetTexture = null;
				Graphics.Blit(downscaleTarget, null as RenderTexture);
			}
		}
	}
}