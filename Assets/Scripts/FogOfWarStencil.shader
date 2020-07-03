﻿Shader "CCB/FogOfWarStencil"
{
    Properties
    {
        _MainTex("Current Frame Vision", 2D) = "white" {}
        ditherTex("Dither", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1" }

        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
		}

        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Blend Zero One

            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag

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

            uniform sampler2D _MainTex;
            uniform sampler2D ditherTex;
            fixed4 _MainTex_TexelSize;
            fixed4 ditherTex_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed visibility = tex2D(_MainTex, i.uv).r;
                fixed dither = tex2D(ditherTex, i.uv * _MainTex_TexelSize.zw * ditherTex_TexelSize.zw).r;
                clip(step(dither, visibility) - 0.5); // Step = 1 when frag visible, 0 otherwise. -0.5 is an arbitrary value to clip non-visible things.
                return 0;
            }

            ENDCG
        }
    }
}