Shader "Custom/WireFrameShader" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _WireframeTex ("Wireframe (RGB)", 2D) = "black" {}
        [HideInInspector] _BodyCount ("Body Count", Int) = 0
        [HideInInspector] _BodyDataTex ("Body Data (R)", 2D) = "black" {}
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha:fade fullforwardshadows vertex:vert
        #pragma target 3.0

        fixed4 _Color;
        sampler2D _MainTex;
        sampler2D _WireframeTex;

        int _BodyCount;
        sampler2D _BodyDataTex;
        float4 _BodyDataTex_TexelSize;

        struct Input {
            fixed4 color            : COLOR;
            float2 uv_MainTex       : TEXCOORD0;
            float2 uv2_WireframeTex : TEXCOORD1;
        };

        void vert (inout appdata_full v) {
            const float AccelerationScale =
                0.05f;
            const float SimulationSofteningLengthSquared =
                10.0f;

            float4 bodyDataTextureCoordinates =
                { 0.0f, 0.0f, 0.0f, 0.0f };

            float3 totalAcceleration = { 0.0f, 0.0f, 0.0f };
            for (int i = 0; i < _BodyCount; ++i) {
                bodyDataTextureCoordinates.x =
                    i % (int) _BodyDataTex_TexelSize.z;
                bodyDataTextureCoordinates.y =
                    i / (int) _BodyDataTex_TexelSize.w;
                bodyDataTextureCoordinates.xy *=
                    _BodyDataTex_TexelSize.xy;

                float3 acceleration =
                    { 0.0f, 0.0f, 0.0f };

                float4 bodyPositionWithMass =
                    tex2Dlod (_BodyDataTex, bodyDataTextureCoordinates);
                float4 positionInWorldSpace =
                    mul(unity_ObjectToWorld, v.vertex);
                float3 universeR =
                    bodyPositionWithMass.xyz - positionInWorldSpace.xyz;

                float distanceSquared =
                    dot (universeR, universeR) + SimulationSofteningLengthSquared;
                float distanceSquaredCube =
                    distanceSquared * distanceSquared * distanceSquared;
                float inverse =
                    1.0f / sqrt (distanceSquaredCube);
                float scale =
                    bodyPositionWithMass.a * inverse;

                acceleration =
                    universeR * scale;
                totalAcceleration +=
                    acceleration;
            }

            v.vertex.y -=
                length (totalAcceleration) * AccelerationScale;
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 mainColor =
                tex2D (_MainTex, IN.uv_MainTex) * IN.color * _Color;
            fixed4 wireframeColor =
                tex2D (_WireframeTex, IN.uv2_WireframeTex);

            o.Albedo =
                mainColor + wireframeColor;
            o.Alpha =
                mainColor.r * wireframeColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

