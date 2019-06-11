// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:32719,y:32712,varname:node_4013,prsc:2|diff-1304-RGB,normal-3147-OUT,emission-712-OUT,transm-5345-RGB,lwrap-7754-RGB,alpha-1256-OUT,clip-2282-OUT,voffset-1620-OUT,tess-4090-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:32164,y:32439,ptovrint:False,ptlb:Cloud Color,ptin:_CloudColor,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:4090,x:32473,y:33189,ptovrint:False,ptlb:Tassellation,ptin:_Tassellation,varname:node_4090,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:6;n:type:ShaderForge.SFN_Multiply,id:712,x:32400,y:32395,varname:node_712,prsc:2|A-1304-RGB,B-7380-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7380,x:32223,y:32621,ptovrint:False,ptlb:Emission Power,ptin:_EmissionPower,varname:node_7380,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.2;n:type:ShaderForge.SFN_Fresnel,id:2595,x:31699,y:32952,varname:node_2595,prsc:2|EXP-7103-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6047,x:31559,y:33172,ptovrint:False,ptlb:Fresnel Power Outr,ptin:_FresnelPowerOutr,varname:node_6047,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4;n:type:ShaderForge.SFN_ValueProperty,id:7103,x:31539,y:33047,ptovrint:False,ptlb:Fresnel Power Inner,ptin:_FresnelPowerInner,varname:node_7103,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.8;n:type:ShaderForge.SFN_Fresnel,id:1945,x:31735,y:33109,varname:node_1945,prsc:2|EXP-6047-OUT;n:type:ShaderForge.SFN_OneMinus,id:8648,x:31890,y:33109,varname:node_8648,prsc:2|IN-1945-OUT;n:type:ShaderForge.SFN_OneMinus,id:4243,x:31829,y:32952,varname:node_4243,prsc:2|IN-2595-OUT;n:type:ShaderForge.SFN_RemapRange,id:1226,x:32044,y:32952,varname:node_1226,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-2706-OUT;n:type:ShaderForge.SFN_RemapRange,id:4395,x:32052,y:33111,varname:node_4395,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-8648-OUT;n:type:ShaderForge.SFN_Clamp01,id:6108,x:32203,y:32952,varname:node_6108,prsc:2|IN-1226-OUT;n:type:ShaderForge.SFN_Multiply,id:2282,x:32358,y:32952,varname:node_2282,prsc:2|A-6108-OUT,B-4395-OUT;n:type:ShaderForge.SFN_Clamp01,id:1256,x:32524,y:32952,varname:node_1256,prsc:2|IN-2282-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:2751,x:31329,y:32578,ptovrint:False,ptlb:CloudTex,ptin:_CloudTex,varname:node_2751,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:66d97dacc9bfb44449b9e3571c6131c6,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4731,x:31707,y:32711,varname:node_4731,prsc:2,tex:66d97dacc9bfb44449b9e3571c6131c6,ntxv:0,isnm:False|UVIN-8813-OUT,TEX-2751-TEX;n:type:ShaderForge.SFN_Add,id:2706,x:31948,y:32797,varname:node_2706,prsc:2|A-4731-R,B-4243-OUT;n:type:ShaderForge.SFN_Multiply,id:1620,x:32175,y:33341,varname:node_1620,prsc:2|A-4731-RGB,B-6197-OUT,C-2484-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6197,x:31887,y:33535,ptovrint:False,ptlb:OffsetPower,ptin:_OffsetPower,varname:node_6197,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.49;n:type:ShaderForge.SFN_TexCoord,id:7256,x:31342,y:32881,varname:node_7256,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:8813,x:31504,y:32826,varname:node_8813,prsc:2|A-3923-OUT,B-7256-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:3923,x:31217,y:32802,ptovrint:False,ptlb:Tiling Noise,ptin:_TilingNoise,varname:node_3923,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Color,id:5345,x:32363,y:32767,ptovrint:False,ptlb:Translucent Color,ptin:_TranslucentColor,varname:node_5345,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5754717,c2:0.5251738,c3:0.4044589,c4:1;n:type:ShaderForge.SFN_Tex2dAsset,id:9357,x:31329,y:32377,ptovrint:False,ptlb:Normal Texture,ptin:_NormalTexture,varname:node_9357,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:dbada663e3650154dad02c85825b30c5,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:6668,x:31831,y:32531,varname:node_6668,prsc:2,tex:dbada663e3650154dad02c85825b30c5,ntxv:0,isnm:False|UVIN-3294-OUT,TEX-9357-TEX;n:type:ShaderForge.SFN_TexCoord,id:7815,x:31526,y:32272,varname:node_7815,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:6621,x:31794,y:32326,varname:node_6621,prsc:2|A-3998-OUT,B-7815-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:3998,x:31506,y:32203,ptovrint:False,ptlb:Tiling Normal Offset,ptin:_TilingNormalOffset,varname:node_3998,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:3294,x:31678,y:32401,varname:node_3294,prsc:2|A-3998-OUT,B-7815-UVOUT;n:type:ShaderForge.SFN_Multiply,id:3147,x:32080,y:32646,varname:node_3147,prsc:2|A-6668-RGB,B-4432-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4432,x:31993,y:32498,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_4432,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.0042;n:type:ShaderForge.SFN_Color,id:7754,x:32188,y:32767,ptovrint:False,ptlb:LightWrappingColor,ptin:_LightWrappingColor,varname:node_7754,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.6857865,c2:0.6714578,c3:0.745283,c4:1;n:type:ShaderForge.SFN_NormalVector,id:2484,x:31866,y:33282,prsc:2,pt:False;proporder:1304-4090-7380-6047-7103-2751-6197-3923-5345-9357-3998-4432-7754;pass:END;sub:END;*/

Shader "Shader Forge/CloudRedo" {
    Properties {
        _CloudColor ("Cloud Color", Color) = (1,1,1,1)
        _Tassellation ("Tassellation", Float ) = 6
        _EmissionPower ("Emission Power", Float ) = 0.2
        _FresnelPowerOutr ("Fresnel Power Outr", Float ) = 4
        _FresnelPowerInner ("Fresnel Power Inner", Float ) = 0.8
        _CloudTex ("CloudTex", 2D) = "white" {}
        _OffsetPower ("OffsetPower", Float ) = 0.49
        _TilingNoise ("Tiling Noise", Float ) = 1
        _TranslucentColor ("Translucent Color", Color) = (0.5754717,0.5251738,0.4044589,1)
        _NormalTexture ("Normal Texture", 2D) = "bump" {}
        _TilingNormalOffset ("Tiling Normal Offset", Float ) = 1
        _Normal ("Normal", Float ) = 0.0042
        _LightWrappingColor ("LightWrappingColor", Color) = (0.6857865,0.6714578,0.745283,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Tessellation.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 5.0
            uniform float4 _LightColor0;
            uniform float4 _CloudColor;
            uniform float _Tassellation;
            uniform float _EmissionPower;
            uniform float _FresnelPowerOutr;
            uniform float _FresnelPowerInner;
            uniform sampler2D _CloudTex; uniform float4 _CloudTex_ST;
            uniform float _OffsetPower;
            uniform float _TilingNoise;
            uniform float4 _TranslucentColor;
            uniform sampler2D _NormalTexture; uniform float4 _NormalTexture_ST;
            uniform float _TilingNormalOffset;
            uniform float _Normal;
            uniform float4 _LightWrappingColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float2 node_8813 = (_TilingNoise*o.uv0);
                float4 node_4731 = tex2Dlod(_CloudTex,float4(TRANSFORM_TEX(node_8813, _CloudTex),0.0,0));
                v.vertex.xyz += (node_4731.rgb*_OffsetPower*v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    return o;
                }
                float Tessellation(TessVertex v){
                    return _Tassellation;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_3294 = (_TilingNormalOffset*i.uv0);
                float3 node_6668 = UnpackNormal(tex2D(_NormalTexture,TRANSFORM_TEX(node_3294, _NormalTexture)));
                float3 normalLocal = (node_6668.rgb*_Normal);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float2 node_8813 = (_TilingNoise*i.uv0);
                float4 node_4731 = tex2D(_CloudTex,TRANSFORM_TEX(node_8813, _CloudTex));
                float node_2282 = (saturate(((node_4731.r+(1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelPowerInner)))*0.5+0.5))*((1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelPowerOutr))*2.0+-1.0));
                clip(node_2282 - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 w = _LightWrappingColor.rgb*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * _TranslucentColor.rgb;
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = (forwardLight+backLight) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuseColor = _CloudColor.rgb;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = (_CloudColor.rgb*_EmissionPower);
/// Final Color:
                float3 finalColor = diffuse + emissive;
                fixed4 finalRGBA = fixed4(finalColor,saturate(node_2282));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Tessellation.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 5.0
            uniform float4 _LightColor0;
            uniform float4 _CloudColor;
            uniform float _Tassellation;
            uniform float _EmissionPower;
            uniform float _FresnelPowerOutr;
            uniform float _FresnelPowerInner;
            uniform sampler2D _CloudTex; uniform float4 _CloudTex_ST;
            uniform float _OffsetPower;
            uniform float _TilingNoise;
            uniform float4 _TranslucentColor;
            uniform sampler2D _NormalTexture; uniform float4 _NormalTexture_ST;
            uniform float _TilingNormalOffset;
            uniform float _Normal;
            uniform float4 _LightWrappingColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float2 node_8813 = (_TilingNoise*o.uv0);
                float4 node_4731 = tex2Dlod(_CloudTex,float4(TRANSFORM_TEX(node_8813, _CloudTex),0.0,0));
                v.vertex.xyz += (node_4731.rgb*_OffsetPower*v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    return o;
                }
                float Tessellation(TessVertex v){
                    return _Tassellation;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float2 node_3294 = (_TilingNormalOffset*i.uv0);
                float3 node_6668 = UnpackNormal(tex2D(_NormalTexture,TRANSFORM_TEX(node_3294, _NormalTexture)));
                float3 normalLocal = (node_6668.rgb*_Normal);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float2 node_8813 = (_TilingNoise*i.uv0);
                float4 node_4731 = tex2D(_CloudTex,TRANSFORM_TEX(node_8813, _CloudTex));
                float node_2282 = (saturate(((node_4731.r+(1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelPowerInner)))*0.5+0.5))*((1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelPowerOutr))*2.0+-1.0));
                clip(node_2282 - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 w = _LightWrappingColor.rgb*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * _TranslucentColor.rgb;
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = (forwardLight+backLight) * attenColor;
                float3 diffuseColor = _CloudColor.rgb;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * saturate(node_2282),0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "Tessellation.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 5.0
            uniform float _Tassellation;
            uniform float _FresnelPowerOutr;
            uniform float _FresnelPowerInner;
            uniform sampler2D _CloudTex; uniform float4 _CloudTex_ST;
            uniform float _OffsetPower;
            uniform float _TilingNoise;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float2 node_8813 = (_TilingNoise*o.uv0);
                float4 node_4731 = tex2Dlod(_CloudTex,float4(TRANSFORM_TEX(node_8813, _CloudTex),0.0,0));
                v.vertex.xyz += (node_4731.rgb*_OffsetPower*v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    return o;
                }
                float Tessellation(TessVertex v){
                    return _Tassellation;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float2 node_8813 = (_TilingNoise*i.uv0);
                float4 node_4731 = tex2D(_CloudTex,TRANSFORM_TEX(node_8813, _CloudTex));
                float node_2282 = (saturate(((node_4731.r+(1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelPowerInner)))*0.5+0.5))*((1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelPowerOutr))*2.0+-1.0));
                clip(node_2282 - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
