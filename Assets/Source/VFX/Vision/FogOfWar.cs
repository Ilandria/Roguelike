using UnityEngine;

namespace CCB.Roguelike
{
	public class FogOfWar : MonoBehaviour
	{
		[SerializeField]
		private Transform playerCameraTransform = null;


		[SerializeField]
		private RenderTexture currentFrameVisionTex = null;

		[SerializeField]
		private ComputeShader visionCompositorShader = null;

		[SerializeField]
		private Material fogMaterial = null;

		// Todo: Move this somewhere more appropriate.
		// Multiple of 16 is due to compute shader thread group size.
		[SerializeField]
		[Tooltip("This must be a multiple of 16.")]
		private int worldSize = 256;

		[SerializeField]
		[Tooltip("Number of fog samples per world unit.")]
		[Range(1, 32)]
		private int fogDetailLevel = 2;

		private int fogQuadScaleId = -1;
		private int currentFrameVisionTexId = -1;
		private int persistentVisionTexId = -1;
		private int worldSpaceCamPosId = -1;
		private int persistentVisionTexSizeId = -1;
		private int currentFrameVisionTexSizeId = -1;
		private int worldSizeId = -1;

		private int addCurrentFrameVisionKernel = -1;

		private RenderTexture persistentVisionTex = null;

		private void Start()
		{
			int fogWidth = worldSize * fogDetailLevel;

			persistentVisionTex = new RenderTexture(fogWidth, fogWidth, 0)
			{
				format = RenderTextureFormat.R8,
				enableRandomWrite = true,
				anisoLevel = 0,
				antiAliasing = 1,
				filterMode = FilterMode.Bilinear
			};
			persistentVisionTex.Create();

			FillFog();

			fogQuadScaleId = Shader.PropertyToID("fogQuadScale");
			currentFrameVisionTexId = Shader.PropertyToID("currentFrameVisionTex");
			persistentVisionTexId = Shader.PropertyToID("persistentVisionTex");
			worldSpaceCamPosId = Shader.PropertyToID("worldSpaceCamPos");
			persistentVisionTexSizeId = Shader.PropertyToID("persistentVisionTexSize");
			currentFrameVisionTexSizeId = Shader.PropertyToID("currentFrameVisionTexSize");
			worldSizeId = Shader.PropertyToID("worldSize");

			addCurrentFrameVisionKernel = visionCompositorShader.FindKernel("AddCurrentFrameVision");

			visionCompositorShader.SetTexture(addCurrentFrameVisionKernel, currentFrameVisionTexId, currentFrameVisionTex);
			visionCompositorShader.SetTexture(addCurrentFrameVisionKernel, persistentVisionTexId, persistentVisionTex);
			visionCompositorShader.SetInt(persistentVisionTexSizeId, fogWidth);
			visionCompositorShader.SetInt(currentFrameVisionTexSizeId, currentFrameVisionTex.width);
			visionCompositorShader.SetFloat(fogQuadScaleId, transform.localScale.x);
			// Todo: Remove hard-coded value.
			visionCompositorShader.SetInt(worldSizeId, worldSize);

			fogMaterial.SetFloat(fogQuadScaleId, transform.localScale.x);
			fogMaterial.SetTexture(persistentVisionTexId, persistentVisionTex);
			// Todo: Remove hard-coded value.
			fogMaterial.SetInt(worldSizeId, worldSize);
		}

		public void FillFog()
		{
			RenderTexture previousTexture = RenderTexture.active;
			RenderTexture.active = persistentVisionTex;
			GL.Clear(true, true, Color.black);
			RenderTexture.active = previousTexture;
		}

		public void ClearFog()
		{
			RenderTexture previousTexture = RenderTexture.active;
			RenderTexture.active = persistentVisionTex;
			GL.Clear(true, true, Color.white);
			RenderTexture.active = previousTexture;
		}

		// This technically means FoW is delayed by a frame... but that's fine.
		private void LateUpdate()
		{
			visionCompositorShader.SetVector(worldSpaceCamPosId, playerCameraTransform.position);
			visionCompositorShader.Dispatch(addCurrentFrameVisionKernel, 1, 1, 1);
		}
	}
}
