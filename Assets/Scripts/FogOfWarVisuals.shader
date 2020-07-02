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
            uniform sampler2D fogTex1;
            fixed4 fogTex1_ST;
            fixed4 fogTex1_TexelSize;
            uniform sampler2D fogTex2;
            fixed4 fogTex2_ST;
            fixed4 fogTex2_TexelSize;
            fixed fogSpeed1;
            fixed fogSpeed2;
            fixed minFogBrightness;
            fixed maxFogBrightness;

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
                fog1uv.x += _Time.x * fogSpeed1;
                fixed3 fog1 = tex2D(fogTex1, fog1uv).rgb;

                fixed2 fog2uv = i.uv * fogTex2_TexelSize.zw * fogTex2_ST.xy;
                fog2uv.x += _Time.x * fogSpeed2;
                fixed3 fog2 = tex2D(fogTex2, fog2uv).rgb;

                return fixed4(lerp(minFogBrightness, maxFogBrightness, fog1 * fog2 * fogDensity), fogDensity);
            }
            ENDCG
        }
    }
}
