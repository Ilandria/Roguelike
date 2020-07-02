using UnityEngine;

namespace CCB.Roguelike
{
	public class FogOfWar : MonoBehaviour
	{
		[SerializeField]
		private RenderTexture persistentVisionTex;

		[SerializeField]
		private Material persistentVisionMat;

		[SerializeField]
		private RenderTexture fogOfWarTex;

		[SerializeField]
		private Material fogOfWarMat;

		private void Start()
		{
			ResetFogOfWar();
		}

		public void ResetFogOfWar()
		{
			RenderTexture previousTexture = RenderTexture.active;
			RenderTexture.active = persistentVisionTex;
			GL.Clear(true, true, Color.black);
			RenderTexture.active = fogOfWarTex;
			GL.Clear(true, true, Color.black);
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