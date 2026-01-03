Shader "Custom/HandsChromaPixel"
{
    Properties
    {
        _MainTex ("Video Texture", 2D) = "white" {}
        _KeyColor ("Key Color", Color) = (0,1,0,1)
        _Threshold ("Chroma Threshold", Range(0,1)) = 0.35
        _Softness ("Chroma Softness", Range(0,0.5)) = 0.08
        _PixelSize ("Pixel Size", Range(1,512)) = 64
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        ZWrite Off
        ZTest Always
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _KeyColor;
            float _Threshold;
            float _Softness;
            float _PixelSize;

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
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // PIXELATION
                float2 pixelUV = floor(i.uv * _PixelSize) / _PixelSize;
                fixed4 col = tex2D(_MainTex, pixelUV);

                // CHROMA KEY
                float dist = distance(col.rgb, _KeyColor.rgb);
                float alpha = smoothstep(_Threshold, _Threshold + _Softness, dist);

                col.a = alpha;
                return col;
            }
            ENDCG
        }
    }
}