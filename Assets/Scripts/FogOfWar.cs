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
		private Material fogOfWarMat = null;

		private void Start()
		{
			FillFog();
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
			fogOfWarMat.SetPass(0);
			Graphics.Blit(persistentVisionTex, fogOfWarTex, fogOfWarMat);

			persistentVisionMat.SetPass(0);
			Graphics.Blit(persistentVisionTex, persistentVisionTex, persistentVisionMat);
		}
	}
}