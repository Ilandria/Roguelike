Shader "CCB/FogOfWar"
{
    Properties
    {
        _MainTex ("Persistent Vision", 2D) = "white" {}
        currentFrameVision("New Vision", 2D) = "white" {}
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
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            uniform sampler2D _MainTex;
            uniform sampler2D currentFrameVision;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed persistentVision = tex2D(_MainTex, i.uv).r;
                fixed newVision = tex2D(currentFrameVision, i.uv).r;
                fixed fogAmount = saturate(1 - persistentVision * 0.25 - newVision);
                clip(fogAmount - 0.1);
                return fixed4(0, 0, 0, fogAmount);
            }
            ENDCG
        }
    }
}
