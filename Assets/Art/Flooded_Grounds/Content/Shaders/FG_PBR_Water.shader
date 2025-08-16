Shader "Flooded_Grounds/PBR_Water_URP" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _Emis ("Self-Ilumination", Range(0,1)) = 0.1
        _Smth ("Smoothness", Range(0,1)) = 0.9
        _Parallax ("Height", Range (0.005, 0.08)) = 0.02
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _BumpMap2 ("Normalmap2", 2D) = "bump" {}
        _BumpLerp ("Normalmap2 Blend", Range(0,1)) = 0.5
        _ParallaxMap ("Heightmap", 2D) = "black" {}
        _ScrollSpeed ("Scroll Speed", float) = 0.2
        _WaveFreq ("Wave Frequency", float) = 20
        _WaveHeight ("Wave Height", float) = 0.1
    }

    SubShader {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
        Pass {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            // Include URP shader libraries
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // Textures & samplers
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_BumpMap); SAMPLER(sampler_BumpMap);
            TEXTURE2D(_BumpMap2); SAMPLER(sampler_BumpMap2);
            TEXTURE2D(_ParallaxMap); SAMPLER(sampler_ParallaxMap);

            float4 _Color;
            float _ScrollSpeed;
            float _WaveFreq;
            float _WaveHeight;
            float _Smth;
            float _Emis;
            float _BumpLerp;
            float _Parallax;

            struct Attributes {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvBump : TEXCOORD1;
                float2 uvBump2 : TEXCOORD2;
                float2 uvParallax : TEXCOORD3;
                float3 viewDirTS : TEXCOORD4; // Tangent space view direction
                float3 worldPos : TEXCOORD5;
                float3 worldNormal : TEXCOORD6;
            };

            // Parallax offset function
            float2 ParallaxOffset(float h, float height, float3 viewDir) {
                h = h * height - height / 2.0;
                float3 v = normalize(viewDir);
                v.z += 0.42;
                return h * (v.xy / v.z);
            }

            Varyings vert(Attributes IN) {
                Varyings OUT;
                
                // Wave animation
                float phase = _Time.y * _WaveFreq;
                float offset = (IN.vertex.x + (IN.vertex.z * 2)) * 8;
                IN.vertex.y += sin(phase + offset) * _WaveHeight;
                
                OUT.pos = TransformObjectToHClip(IN.vertex);
                OUT.uv = IN.uv;
                OUT.uvBump = IN.uv;
                OUT.uvBump2 = IN.uv;
                OUT.uvParallax = IN.uv;
                OUT.worldPos = TransformObjectToWorld(IN.vertex).xyz;
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normal);

                // Calculate tangent space view direction
                float3 worldTangent = TransformObjectToWorldDir(IN.tangent.xyz);
                float3 worldBitangent = cross(OUT.worldNormal, worldTangent) * IN.tangent.w;
                float3 worldViewDir = GetWorldSpaceViewDir(OUT.worldPos);
                
                float3x3 tangentToWorld = float3x3(worldTangent, worldBitangent, OUT.worldNormal);
                OUT.viewDirTS = mul(tangentToWorld, worldViewDir);

                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target {
                // Scrolling UVs
                float scrollX = _ScrollSpeed * _Time.y;
                float scrollY = (_ScrollSpeed * _Time.y) * 0.5;
                float scrollX2 = (1 - _ScrollSpeed) * _Time.y;
                float scrollY2 = (1 - _ScrollSpeed * _Time.y) * 0.5;

                // Parallax mapping
                float2 parallaxUV = IN.uvParallax + float2(scrollX * 0.2, scrollY * 0.2);
                float h = SAMPLE_TEXTURE2D(_ParallaxMap, sampler_ParallaxMap, parallaxUV).r;
                float2 offset = ParallaxOffset(h, _Parallax, IN.viewDirTS);

                // Sample textures with offset and scrolling
                float2 mainUV = IN.uv + offset + float2(scrollX, scrollY);
                float2 bumpUV1 = IN.uvBump + offset + float2(scrollX, scrollY);
                float2 bumpUV2 = IN.uvBump + offset + float2(scrollX2, scrollY2);

                float4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, mainUV);
                float3 normal1 = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, bumpUV1));
                float3 normal2 = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, bumpUV2));
                float3 normal3 = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap2, sampler_BumpMap2, IN.uvBump2));

                // Combine normals
                float3 combinedNormal = normal1 + normal2 * float3(1, 1, 0);
                float3 finalNormal = normalize(lerp(combinedNormal, normal3, _BumpLerp));

                // Convert normal to world space
                float3 worldTangent = normalize(cross(IN.worldNormal, float3(0, 0, 1)));
                float3 worldBitangent = normalize(cross(IN.worldNormal, worldTangent));
                float3x3 TBN = float3x3(worldTangent, worldBitangent, IN.worldNormal);
                float3 worldNormal = normalize(mul(finalNormal, TBN));

                // Simple lighting calculation
                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);
                float NdotL = saturate(dot(worldNormal, lightDir));
                
                // Albedo and emission
                float3 albedo = mainTex.rgb * _Color.rgb;
                float3 emission = albedo * _Emis;
                
                // Simple diffuse + ambient + emission
                float3 diffuse = albedo * mainLight.color * NdotL;
                float3 ambient = albedo * 0.2;
                float3 finalColor = diffuse + ambient + emission;

                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Forward"
}