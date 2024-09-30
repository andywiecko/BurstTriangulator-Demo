Shader "Custom/TriMesh"
{
    Properties
    {
        _AlbedoColor ("Albedo", Color) = (1, 1, 1, 1)
        _LineColor ("Line Color", Color) = (0, 0, 0, 1)
        _LineWidth ("Line Width", Range(0.001, 1)) = 0.5
        _LineSmoothing ("Line Smoothing", Range(0, 10)) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 bar : TEXCOORD0;
            };

            float _LineWidth;
            float4 _LineColor;
            float4 _AlbedoColor;
            float _LineSmoothing;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.bar = v.uv2;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float3 bar;
                bar.xy = i.bar;
                bar.z = 1 - bar.x - bar.y;
                float3 deltas = fwidth(bar);
                float3 smoothing = deltas * _LineSmoothing;
                float3 thickness = deltas * _LineWidth;
                bar = smoothstep(thickness, thickness + smoothing, bar);
                float minBar = min(bar.x, min(bar.y, bar.z));
                return lerp(_LineColor, _AlbedoColor, minBar);
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}