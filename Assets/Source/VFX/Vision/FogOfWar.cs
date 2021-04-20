using UnityEngine;

namespace CCB.Roguelike
{
	public class FogOfWar : MonoBehaviour
	{
		[SerializeField]
		private RenderTexture persistentVisionTex = null;

		[SerializeField]
		private Material persistentVisionMat = null;

		[SerializeField]
		private Material fogMaterial = null;

		private int stencilQuadScaleId = -1;
		private int fogQuadScaleId = -1;
		private int stencilFogScaleRatioId = -1;

		private void Start()
		{
			FillFog();

			fogQuadScaleId = Shader.PropertyToID("fogQuadScale"); ;
			stencilQuadScaleId = Shader.PropertyToID("stencilQuadScale");
			stencilFogScaleRatioId = Shader.PropertyToID("stencilFogScaleRatio");
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
			fogMaterial.SetFloat(fogQuadScaleId, transform.localScale.x);
			fogMaterial.SetFloat(stencilQuadScaleId, 256);
			fogMaterial.SetFloat(stencilFogScaleRatioId, 256 / transform.localScale.x);

			persistentVisionMat.SetPass(0);
			Graphics.Blit(persistentVisionTex, persistentVisionTex, persistentVisionMat);
		}
	}
}