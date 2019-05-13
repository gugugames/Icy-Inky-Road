Shader "Toon/ToonShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Ambient("Ambient", Color) = (0.4, 0.4, 0.4, 1.0)
		_Specular("Specular", Color) = (0.9, 0.9, 0.9, 1.0)
		_Glossiness("Glossiness", Float) = 32.0
		_Rim("Rim", Color) = (1.0, 1.0, 1.0, 1.0)
		_Rimness("Rimness", Float) = 0.716
		_RimThreshold("Rim Threshold", Float) = 0.1
	}
	SubShader
	{
		Tags { 
		"LightMode" = "ForwardBase"
		"PassFlags" = "OnlyDirectional"
		"RenderType"="Opaque" 
		}
		LOD 100

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
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldNormal : NORMAL;
				float3 viewDir : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			fixed4 _Ambient;
			fixed4 _Specular;
			float _Glossiness;
			fixed4 _Rim;
			float _Rimness;
			float _RimThreshold;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = WorldSpaceViewDir(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 normal = normalize(i.worldNormal);
				float ndotl = dot(normal, _WorldSpaceLightPos0);
				float diffuse = smoothstep(0, 0.1, ndotl);
				
				float3 viewDir = normalize(i.viewDir);
				float3 halfway = normalize(_WorldSpaceLightPos0 + viewDir);
				float ndoth = dot(normal, halfway);
				float specular = smoothstep(0.005, 0.01, pow(ndoth * diffuse, _Glossiness * _Glossiness)) * _Specular;

				float4 rim = smoothstep(_Rimness - 0.01, _Rimness + 0.01, (1 - dot(viewDir, normal)) * pow(ndotl, _RimThreshold)) * _Rim;
				fixed4 col = tex2D(_MainTex, i.uv) * _Color * diffuse;
				return col + _Ambient + specular + rim;
			}
			ENDCG
		}
	}
}
