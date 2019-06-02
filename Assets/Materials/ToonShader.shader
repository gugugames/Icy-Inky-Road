/*
Toon/ToonShader
Cartoon Rendering을 위한 shader.
Two tone mapping, Ambient Light을 이용한 GI, Specular Reflection, Rim Lighting 등을 제공한다. 
*/

Shader "Toon/ToonShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Ambient("Ambient", Range(0.0, 1.0)) = 0.0
		_Specular("Specular", Range(0.0, 32.0)) = 0.0
		_Rim("Rim", Range(0.0, 1.0)) = 0.0
	}
	SubShader
	{
		Tags { 
		"LightMode" = "ForwardBase"
		"PassFlags" = "OnlyDirectional"
		"RenderType" = "Opaque" 
		}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"	
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float3 view : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			float _Ambient;
			float _Specular;
			float _Rim;
			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.view = WorldSpaceViewDir(v.vertex);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target{
				fixed4 col = _Color * tex2D(_MainTex, i.uv);

				float3 normal = normalize(i.normal);
				float3 view = normalize(i.view);
				float3 halfway = normalize(_WorldSpaceLightPos0 + view); // Blinn-Phong model

				float ndotl = dot(normal, _WorldSpaceLightPos0);
				float ndoth = dot(normal, halfway);
				
				float ambient = _Ambient; // ambient light intensity, calculated by multiplying ambient coefficient
				float diffuse = smoothstep(0, 0.1, ndotl); // diffuse light intensity, calculated by ndotl
				float specular = smoothstep(0.005, 0.01, pow(ndoth, _Specular * _Specular)); // specular light intensity
				float rim = (1 - dot(view, normal)) * _Rim; // rim light intensity. not toon rendering, but looks good
				return col * (diffuse + ambient + specular + rim);
			}
			ENDCG
		}
	}
}
