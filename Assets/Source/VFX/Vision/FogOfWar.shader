Shader "CCB/FogOfWar"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		ditherTex("Dither Texture", 2D) = "white" {}
		backgroundTex ("Background Texture", 2D) = "black" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Cull Off ZWrite Off ZTest Always
		Stencil
        {
            Ref 1
            Comp NotEqual
            Pass Keep
		}
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD1;
			};

			uniform sampler2D _MainTex, backgroundTex;
			uniform fixed4 backgroundTex_ST;
			uniform fixed4 _MainTex_TexelSize;
			uniform sampler2D ditherTex;
			fixed4 ditherTex_TexelSize;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 fog = tex2D(_MainTex, i.uv);
				fixed2 backUV = i.screenPos.xy / i.screenPos.w;
				backUV.x = backUV.x * (_ScreenParams.x / _ScreenParams.y);
				fixed3 background = tex2D(backgroundTex, backUV).rgb;

				fixed3 col = fog.rgb;
				col += saturate(background - max(max(col.r, col.g), col.b));
				fixed dither = tex2D(ditherTex, i.uv * 128 * ditherTex_TexelSize.zw).r; // Todo: Remove magic number.
				fixed fogDensity = step(dither, fog.a);

				return fixed4(col, fogDensity);
			}
			ENDCG
		}
	}
}
