Shader "Custom/DirectionalFog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (0.5, 0.5, 0.5, 1)
        _FogDensity ("Fog Density", Float) = 0.1
        _VerticalFogStart ("Vertical Fog Start", Float) = 10.0
        _VerticalFogEnd ("Vertical Fog End", Float) = 50.0
        _HorizontalFogThreshold ("Horizontal Fog Threshold", Float) = 100.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

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
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _FogColor;
            float _FogDensity;
            float _VerticalFogStart;
            float _VerticalFogEnd;
            float _HorizontalFogThreshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float CalculateFog(float3 worldPos)
            {
                // Distance from the camera
                float horizontalDist = length(worldPos.xz);
                float verticalDist = abs(worldPos.y);

                // Horizontal fog
                float horizontalFog = smoothstep(0.0, _HorizontalFogThreshold, horizontalDist);

                // Vertical fog
                float verticalFog = smoothstep(_VerticalFogStart, _VerticalFogEnd, verticalDist);

                // Combine vertical and horizontal fog
                return max(horizontalFog, verticalFog);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the main texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // Calculate fog amount
                float fogFactor = CalculateFog(i.worldPos) * _FogDensity;

                // Blend the fog color
                return lerp(col, _FogColor, fogFactor);
            }
            ENDCG
        }
    }
}
