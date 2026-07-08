Shader "Weather/SkyGradient"
{
    Properties
    {
        _BottomColor ("Horizon Color", Color) = (0.65, 0.82, 0.95, 1)
        _TopColor ("Zenith Color", Color) = (0.15, 0.35, 0.75, 1)
        _BottomHeight ("Horizon World Y", Float) = -5
        _TopHeight ("Zenith World Y", Float) = 5
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry-100" }
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float worldY : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BottomColor;
                float4 _TopColor;
                float _BottomHeight;
                float _TopHeight;
            CBUFFER_END

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(positionWS);
                OUT.worldY = positionWS.y;
                return OUT;
            }

            float4 Frag(Varyings IN) : SV_Target
            {
                float t = saturate((IN.worldY - _BottomHeight) / max(_TopHeight - _BottomHeight, 0.0001));
                return lerp(_BottomColor, _TopColor, t);
            }
            ENDHLSL
        }
    }
}
