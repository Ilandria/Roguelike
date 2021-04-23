using UnityEngine;

namespace CCB.Roguelike
{
	public class FogOfWar : MonoBehaviour
	{
		[SerializeField]
		private Transform playerCameraTransform = null;

		[SerializeField]
		private RenderTexture persistentVisionTex = null;

		[SerializeField]
		private RenderTexture currentFrameVisionTex = null;

		[SerializeField]
		private ComputeShader visionCompositorShader = null;

		[SerializeField]
		private Material fogMaterial = null;

		private int fogQuadScaleId = -1;
		private int currentFrameVisionTexId = -1;
		private int persistentVisionTexId = -1;
		private int worldSpaceCamPosId = -1;
		private int persistentVisionTexSizeId = -1;
		private int currentFrameVisionTexSizeId = -1;
		private int worldSizeId = -1;

		private int addCurrentFrameVisionKernel = -1;

		private void Start()
		{
			persistentVisionTex = new RenderTexture(persistentVisionTex.width, persistentVisionTex.width, persistentVisionTex.depth)
			{
				format = persistentVisionTex.format,
				enableRandomWrite = true,
				anisoLevel = persistentVisionTex.anisoLevel,
				antiAliasing = persistentVisionTex.antiAliasing,
				filterMode = persistentVisionTex.filterMode
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
			visionCompositorShader.SetInt(persistentVisionTexSizeId, persistentVisionTex.width);
			visionCompositorShader.SetInt(currentFrameVisionTexSizeId, currentFrameVisionTex.width);
			// Todo: Remove hard-coded value.
			visionCompositorShader.SetInt(worldSizeId, 256);

			fogMaterial.SetTexture(persistentVisionTexId, persistentVisionTex);
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
			// Todo: Only update these if they've changed.
			// Todo: Make the vision stencil camera/object local to the player.
			fogMaterial.SetFloat(fogQuadScaleId, transform.localScale.x / 2);
			//persistentVisionMat.SetFloat(fogQuadScaleId, transform.localScale.x / 2);

			//persistentVisionMat.SetPass(0);
			//Graphics.Blit(persistentVisionTex, persistentVisionTex, persistentVisionMat);
			visionCompositorShader.SetVector(worldSpaceCamPosId, playerCameraTransform.position);
			visionCompositorShader.Dispatch(addCurrentFrameVisionKernel, 1, 1, 1);
		}
	}
}
