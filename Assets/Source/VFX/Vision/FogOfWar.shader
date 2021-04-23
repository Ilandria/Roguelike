Shader "CCB/FogOfWar"
{
	Properties
	{
		[NoScaleOffset] fogTex1 ("Fog Texture 1", 2D) = "black" { }
		[NoScaleOffset] fogTex2 ("Fog Texture 2", 2D) = "black" { }
		[PowerSlider(10.0)] fogScale1 ("Fog Scale 1", Range(0.001, 1)) = 0.03
		[PowerSlider(10.0)] fogScale2 ("Fog Scale 2", Range(0.001, 1)) = 0.01
		fogSpeed1 ("Fog Speed 1", Float) = 0.1
		fogSpeed2 ("Fog Speed 2", Float) = 0.2

		[NoScaleOffset] _FlowMap ("Flow (RG, A noise)", 2D) = "black" { }
		_UJump ("U jump per phase", Range(-0.25, 0.25)) = 0.25
		_VJump ("V jump per phase", Range(-0.25, 0.25)) = 0.25
		_Tiling ("Tiling", Float) = 1
		_Speed ("Speed", Float) = 1
		_FlowStrength ("Flow Strength", Float) = 1
		_FlowOffset ("Flow Offset", Float) = 0

		backgroundTex ("Background Texture", 2D) = "black" { }
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
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
			#include "Flow.cginc"

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

			uniform sampler2D fogTex1, fogTex2, _FlowMap, backgroundTex, persistentVisionTex;
			uniform fixed4 fogTex1_TexelSize, fogTex2_TexelSize, backgroundTex_ST;
			uniform fixed fogScale1, fogScale2, fogSpeed1, fogSpeed2;
			uniform float _UJump, _VJump, _Tiling, _Speed, _FlowStrength, _FlowOffset;
			uniform float fogQuadScale;
			uniform uint worldSize;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// Todo: Get vision textures to the correct size and to line up.
				float2 uv = (i.uv - 0.5) / fogQuadScale + 0.5 + _WorldSpaceCameraPos.xy / worldSize;

				fixed persistentVision = tex2D(persistentVisionTex, uv).r;

				fixed2 backUV = i.screenPos.xy / i.screenPos.w;
				backUV.x = backUV.x * (_ScreenParams.x / _ScreenParams.y);
				fixed3 backgroundColour = tex2D(backgroundTex, backUV + _WorldSpaceCameraPos.xy / worldSize).rgb;

				// Fog layer 1.
				fixed2 fog1uv = i.uv * fogTex1_TexelSize.zw * fogScale1;
				float3 flow = tex2D(_FlowMap, fog1uv).rgb;
				flow.xy = flow.xy * 2 - 1;
				flow *= _FlowStrength;
				float noise = tex2D(_FlowMap, fog1uv).a;
				float time = _Time.y * _Speed + noise;
				float2 jump = float2(_UJump, _VJump);

				float3 uvw = FlowUVW(fog1uv, flow.xy, jump, _FlowOffset, _Tiling, time);
				// Todo: Cleanup magic number.
				fixed4 tex = tex2D(fogTex1, uvw.xy + _WorldSpaceCameraPos.xy / 8) * uvw.z;

				// Todo: Cleanup magic number.
				fixed3 fogColour = tex.rgb * fixed3(0.192, 0.128, 0.192) * 0.1;

				// Fog layer 2.
				fixed2 fog2uv = i.uv * fogTex2_TexelSize.zw * fogScale2;
				flow = tex2D(_FlowMap, fog2uv).rgb;
				flow.xy = flow.xy * 2 - 1;
				flow *= _FlowStrength;
				noise = tex2D(_FlowMap, fog2uv).a;
				time = _Time.y * _Speed + noise;
				jump = float2(_UJump, _VJump);

				uvw = FlowUVW(fog2uv, flow.xy, jump, _FlowOffset, _Tiling, time);
				// Todo: Cleanup magic number.
				tex = tex2D(fogTex2, uvw.xy + _WorldSpaceCameraPos.xy / 8) * uvw.z;

				// Todo: Cleanup magic number.
				fogColour += tex.rgb * fixed3(0.256, 0.384, 0.384) * 0.1;

				fixed3 col = fogColour.rgb;
				col += backgroundColour * (1 - persistentVision);
				fixed fogDensity = saturate(1 - (persistentVision - 0.1));
				return fixed4(col, fogDensity);
			}
			ENDCG

		}
	}
}
