Shader "CCB/FogOfWarCompositor"
{
	Properties
	{
		_MainTex ("Persistent Vision", 2D) = "white" {}
		currentFrameVision("New Vision", 2D) = "white" {}

		[NoScaleOffset] fogTex1 ("Fog Texture 1", 2D) = "black" {}
		[NoScaleOffset] fogTex2 ("Fog Texture 2", 2D) = "black" {}
		[PowerSlider(10.0)] fogScale1 ("Fog Scale 1", Range(0.001, 1)) = 0.03
		[PowerSlider(10.0)] fogScale2 ("Fog Scale 2", Range(0.001, 1)) = 0.01
		fogSpeed1 ("Fog Speed 1", Float) = 0.1
		fogSpeed2 ("Fog Speed 2", Float) = 0.2

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
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Flow.cginc"

			uniform sampler2D _MainTex, fogTex1, fogTex2, currentFrameVision, _FlowMap;
			uniform fixed4 fogTex1_TexelSize, fogTex2_TexelSize;
			uniform fixed fogScale1, fogScale2, fogSpeed1, fogSpeed2;
			uniform float _UJump, _VJump, _Tiling, _Speed, _FlowStrength, _FlowOffset;

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// Fog layer 1.
				fixed2 fog1uv = i.uv * fogTex1_TexelSize.zw * fogScale1;
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

				fixed3 fog1 = (texA.rgb + texB.rgb) * fixed3(0.128, 0.064, 0.064); // Todo: Cleanup magic number.

				// Fog layer 2.
				fixed2 fog2uv = i.uv * fogTex2_TexelSize.zw * fogScale2;
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

				fixed3 fog2 = (texA.rgb + texB.rgb) * fixed3(0.064, 0.128, 0.256); // Todo: Cleanup magic number.

				// Final output.
				fixed persistentVision = tex2D(_MainTex, i.uv).r;
				fixed newVision = tex2D(currentFrameVision, i.uv).r;
				fixed3 fogColour = fog1 + fog2;
				fixed maxColour = max(max(fogColour.r, fogColour.g), fogColour.b);
				fixed fogDensity = saturate(1 - persistentVision * (1 - maxColour) - newVision);//saturate(1 - persistentVision * (1 - maxColour) - newVision); // 1 - maxColour makes wisps appear in persistent area.
				fogDensity = saturate(fogDensity + maxColour);
				return fixed4(fogColour, fogDensity);
			}
			ENDCG
		}
	}
}
