Shader "Custom/ClayShader"
{
    Properties
    {
        [Header(Base Properties)]
        _BaseColor ("Base Color", Color) = (0.8, 0.6, 0.4, 1.0)
        _BaseMap ("Base Texture", 2D) = "white" {}
        
        [Header(Surface Properties)]
        _Smoothness ("Smoothness", Range(0.0, 1.0)) = 0.3
        _Metallic ("Metallic", Range(0.0, 1.0)) = 0.0
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalScale ("Normal Scale", Float) = 1.0
        
        [Header(Clay Properties)]
        _SubsurfaceColor ("Subsurface Color", Color) = (1.0, 0.8, 0.6, 1.0)
        _SubsurfaceStrength ("Subsurface Strength", Range(0.0, 2.0)) = 0.8
        _ThicknessMap ("Thickness Map", 2D) = "white" {}
        _ThicknessScale ("Thickness Scale", Range(0.0, 2.0)) = 1.0
        
        [Header(Rim Lighting)]
        _RimColor ("Rim Color", Color) = (1.0, 0.9, 0.7, 1.0)
        _RimPower ("Rim Power", Range(0.1, 8.0)) = 2.0
        _RimIntensity ("Rim Intensity", Range(0.0, 2.0)) = 0.5
        
        [Header(Ambient)]
        _AmbientOcclusion ("Ambient Occlusion", 2D) = "white" {}
        _AOStrength ("AO Strength", Range(0.0, 1.0)) = 1.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
            "Queue" = "Geometry"
        }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // URP keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseMap_ST;
                float4 _NormalMap_ST;
                float4 _SubsurfaceColor;
                float4 _RimColor;
                float4 _ThicknessMap_ST;
                float4 _AmbientOcclusion_ST;
                float _Smoothness;
                float _Metallic;
                float _NormalScale;
                float _SubsurfaceStrength;
                float _ThicknessScale;
                float _RimPower;
                float _RimIntensity;
                float _AOStrength;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_ThicknessMap);
            SAMPLER(sampler_ThicknessMap);
            TEXTURE2D(_AmbientOcclusion);
            SAMPLER(sampler_AmbientOcclusion);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                float2 lightmapUV : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 tangentWS : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 5);
                float4 shadowCoord : TEXCOORD6;
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.normalWS = normalInput.normalWS;
                output.tangentWS = float4(normalInput.tangentWS, input.tangentOS.w);
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);

                OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
                OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

                output.shadowCoord = GetShadowCoord(vertexInput);

                return output;
            }

            // Custom clay lighting function
            float3 ClayLighting(float3 albedo, float3 normalWS, float3 lightDir, float3 viewDir, 
                               float3 lightColor, float3 subsurfaceColor, float subsurfaceStrength, float thickness)
            {
                // Standard diffuse
                float NdotL = saturate(dot(normalWS, lightDir));
                float3 diffuse = albedo * lightColor * NdotL;

                // Subsurface scattering approximation
                float3 lightDirInverted = -lightDir;
                float subsurfaceDot = pow(saturate(dot(viewDir, lightDirInverted)), 2);
                float3 subsurface = subsurfaceColor * lightColor * subsurfaceDot * subsurfaceStrength * thickness;

                // Oren-Nayar diffuse for clay-like roughness
                float VdotN = saturate(dot(viewDir, normalWS));
                float roughness = 0.6; // Clay has some roughness
                float A = 1.0 - 0.5 * (roughness * roughness) / (roughness * roughness + 0.57);
                float B = 0.45 * (roughness * roughness) / (roughness * roughness + 0.09);
                
                float3 lightPlane = normalize(lightDir - normalWS * NdotL);
                float3 viewPlane = normalize(viewDir - normalWS * VdotN);
                float cosAzimuth = saturate(dot(lightPlane, viewPlane));
                
                float orenNayar = A + B * cosAzimuth * sin(max(acos(NdotL), acos(VdotN))) * tan(min(acos(NdotL), acos(VdotN)));
                diffuse *= orenNayar;

                return diffuse + subsurface;
            }

            float4 frag(Varyings input) : SV_Target
            {
                // Sample textures
                float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                float3 albedo = baseMap.rgb * _BaseColor.rgb;
                
                float3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv), _NormalScale);
                float thickness = SAMPLE_TEXTURE2D(_ThicknessMap, sampler_ThicknessMap, input.uv).r * _ThicknessScale;
                float ao = lerp(1.0, SAMPLE_TEXTURE2D(_AmbientOcclusion, sampler_AmbientOcclusion, input.uv).r, _AOStrength);

                // Transform normal to world space
                float3 bitangentWS = cross(input.normalWS, input.tangentWS.xyz) * input.tangentWS.w;
                float3x3 tangentToWorld = float3x3(input.tangentWS.xyz, bitangentWS, input.normalWS);
                float3 normalWS = normalize(mul(normalTS, tangentToWorld));

                float3 viewDirWS = normalize(input.viewDirWS);

                // Main light
                Light mainLight = GetMainLight(input.shadowCoord);
                float3 lightColor = mainLight.color * mainLight.distanceAttenuation * mainLight.shadowAttenuation;
                
                float3 finalColor = ClayLighting(albedo, normalWS, mainLight.direction, viewDirWS, 
                                               lightColor, _SubsurfaceColor.rgb, _SubsurfaceStrength, thickness);

                // Additional lights
                #ifdef _ADDITIONAL_LIGHTS
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
                {
                    Light light = GetAdditionalLight(lightIndex, input.positionWS);
                    float3 additionalLightColor = light.color * light.distanceAttenuation * light.shadowAttenuation;
                    finalColor += ClayLighting(albedo, normalWS, light.direction, viewDirWS,
                                             additionalLightColor, _SubsurfaceColor.rgb, _SubsurfaceStrength * 0.5, thickness);
                }
                #endif

                // Rim lighting
                float rimDot = 1.0 - saturate(dot(normalWS, viewDirWS));
                float3 rimLighting = _RimColor.rgb * pow(rimDot, _RimPower) * _RimIntensity;

                // Global illumination
                float3 bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, normalWS);
                float3 giColor = bakedGI * albedo * ao;

                // Specular reflection
                float3 reflectVector = reflect(-viewDirWS, normalWS);
                float NdotV = saturate(dot(normalWS, viewDirWS));
                float3 F0 = lerp(float3(0.04, 0.04, 0.04), albedo, _Metallic);
                float3 fresnel = F0 + (1.0 - F0) * pow(1.0 - NdotV, 5.0);
                
                float3 specular = fresnel * _Smoothness * mainLight.color * 
                                pow(saturate(dot(normalWS, normalize(mainLight.direction + viewDirWS))), 
                                    (1.0 - _Smoothness) * 128.0) * mainLight.shadowAttenuation;

                // Combine all lighting
                finalColor += giColor + rimLighting + specular;
                finalColor *= ao;

                return float4(finalColor, _BaseColor.a);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            // Shadow caster 키워드
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            // LerpWhiteTo 함수를 직접 정의
            float3 LerpWhiteTo(float3 b, float t)
            {
                float oneMinusT = 1.0 - t;
                return float3(oneMinusT, oneMinusT, oneMinusT) + b * t;
            }

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            float4 GetShadowPositionHClip(Attributes input)
            {
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                    float3 lightDirectionWS = normalize(_LightPosition - positionWS);
                #else
                    float3 lightDirectionWS = _LightDirection;
                #endif

                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

                #if UNITY_REVERSED_Z
                positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #else
                positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #endif

                return positionCS;
            }

            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output;
                output.positionCS = GetShadowPositionHClip(input);
                return output;
            }

            float4 ShadowPassFragment(Varyings input) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
} 