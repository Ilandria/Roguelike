Shader "CCB/FogOfWarVisuals"
{
	Properties
	{
		_MainTex ("Fog of War", 2D) = "white" {}
		fogTex1 ("Fog Texture 1", 2D) = "black" {}
		fogTex2 ("Fog Texture 2", 2D) = "black" {}
		fogSpeed1 ("Fog Speed 1", Float) = 0.1
		fogSpeed2 ("Fog Speed 2", Float) = 0.2
		minFogBrightness ("Min Fog Brightness", Float) = 0.02
		maxFogBrightness ("Max Fog Brightness", Float) = 0.2

		[NoScaleOffset] _FlowMap ("Flow (RG, A noise)", 2D) = "black" {}
		_UJump ("U jump per phase", Range(-0.25, 0.25)) = 0.25
		_VJump ("V jump per phase", Range(-0.25, 0.25)) = 0.25
		_Tiling ("Tiling", Float) = 1
		_Speed ("Speed", Float) = 1
		_FlowStrength ("Flow Strength", Float) = 1
		_FlowOffset ("Flow Offset", Float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }

		Cull Off ZWrite Off ZTest Always

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
			};

			uniform sampler2D _MainTex, fogTex1, fogTex2;
			uniform fixed4 fogTex1_ST, fogTex1_TexelSize, fogTex2_ST, fogTex2_TexelSize;
			uniform fixed fogSpeed1, fogSpeed2, minFogBrightness, maxFogBrightness;

			uniform sampler2D _FlowMap;
			uniform float _UJump, _VJump, _Tiling, _Speed, _FlowStrength, _FlowOffset;

			float3 UnpackDerivativeHeight (float4 textureData)
			{
				float3 dh = textureData.agb;
				dh.xy = dh.xy * 2 - 1;
				return dh;
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed fogDensity = tex2D(_MainTex, i.uv).a;
				fixed2 fog1uv = i.uv * fogTex1_TexelSize.zw * fogTex1_ST.xy;
				fixed2 fog2uv = i.uv * fogTex2_TexelSize.zw * fogTex2_ST.xy;

				// Fog layer 1
				float3 flow = tex2D(_FlowMap, fog1uv).rgb;
				flow.xy = flow.xy * 2 - 1;
				flow *= _FlowStrength;
				float noise = tex2D(_FlowMap, fog1uv).a;
				float time = _Time.y * _Speed + noise;
				float2 jump = float2(_UJump, _VJump);

				float3 uvwA = FlowUVW(fog1uv, flow.xy, jump, _FlowOffset, _Tiling, time, false);
				float3 uvwB = FlowUVW(fog1uv, flow.xy, jump, _FlowOffset, _Tiling, time, true);

				fixed4 texA = tex2D(fogTex1, uvwA.xy) * uvwA.z;
				fixed4 texB = tex2D(fogTex1, uvwB.xy) * uvwB.z;

				fixed3 fog1 = texA.rgb + texB.rgb;

				// Fog layer 2
				flow = tex2D(_FlowMap, fog2uv).rgb;
				flow.xy = flow.xy * 2 - 1;
				flow *= _FlowStrength;
				noise = tex2D(_FlowMap, fog2uv).a;
				time = _Time.y * _Speed + noise;
				jump = float2(_UJump, _VJump);

				uvwA = FlowUVW(fog2uv, flow.xy, jump, _FlowOffset, _Tiling, time, false);
				uvwB = FlowUVW(fog2uv, flow.xy, jump, _FlowOffset, _Tiling, time, true);

				texA = tex2D(fogTex2, uvwA.xy) * uvwA.z;
				texB = tex2D(fogTex2, uvwB.xy) * uvwB.z;

				fixed3 fog2 = texA.rgb + texB.rgb;



				



				fixed3 fogColour = fog1 * fog2;

				// TODO: No if statement! This is just test code.
				if (fogDensity < 1.0)
				{
					fogDensity += (fogColour.r + fogColour.g + fogColour.b) / 3.0;
				}

				fogColour = lerp(minFogBrightness, maxFogBrightness, fogColour);
				return fixed4(fogColour, fogDensity);
			}
			ENDCG
		}
	}
}
