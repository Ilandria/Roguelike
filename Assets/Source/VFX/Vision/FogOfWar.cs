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
		private RenderTexture fogOfWarTex = null;

		[SerializeField]
		private Material fogOfWarCompositor = null;

		[SerializeField]
		private Material fogMaterial = null;

		private int visionTexWidthId = -1;
		private int fogScaleId = -1;
		private int visionWidthFogScaleRatioId = -1;

		private void Start()
		{
			FillFog();

			fogScaleId = Shader.PropertyToID("fogScale"); ;
			visionTexWidthId = Shader.PropertyToID("visionTexWidth");
			visionWidthFogScaleRatioId = Shader.PropertyToID("visionWidthFogScaleRatio");
		}

		public void FillFog()
		{
			RenderTexture previousTexture = RenderTexture.active;
			RenderTexture.active = persistentVisionTex;
			GL.Clear(true, true, Color.black);
			RenderTexture.active = fogOfWarTex;
			GL.Clear(true, true, Color.black);
			RenderTexture.active = previousTexture;
		}

		public void ClearFog()
		{
			RenderTexture previousTexture = RenderTexture.active;
			RenderTexture.active = persistentVisionTex;
			GL.Clear(true, true, Color.white);
			RenderTexture.active = fogOfWarTex;
			GL.Clear(true, true, Color.clear);
			RenderTexture.active = previousTexture;
		}

		// This technically means FoW is delayed by a frame... but that's fine.
		private void LateUpdate()
		{
			// Todo: Only update these if they've changed.
			fogMaterial.SetFloat(fogScaleId, transform.localScale.x);
			fogMaterial.SetFloat(visionTexWidthId, persistentVisionTex.width);
			fogMaterial.SetFloat(visionWidthFogScaleRatioId, persistentVisionTex.width / transform.localScale.x);

			fogOfWarCompositor.SetPass(0);
			Graphics.Blit(persistentVisionTex, fogOfWarTex, fogOfWarCompositor);

			persistentVisionMat.SetPass(0);
			Graphics.Blit(persistentVisionTex, persistentVisionTex, persistentVisionMat);
		}
	}
}