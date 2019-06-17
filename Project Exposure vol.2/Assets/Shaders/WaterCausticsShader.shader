Shader "Custom/WaterCausticsShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_NoiseSpeed("NoiseSpeed", float) = 1
		_NoiseAmplifier("NoiseAmplifier", float) = 0.2
		_NoiseFrequency("NoiseFrequency", float) = 0.1

		_WaveColorAmplifier("WaveColorAmplifier", float) = 0.2

		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_Uvspeed("Uv Changing Speed", Range(0,10)) = 0.05
		_UvMoveDist("Uv Move Distance", Range(0,1)) = 0.05
		_UvFrequency("Uv Frequency", Range(0, 100)) = 1.0
	}
		SubShader
		{
			Tags { "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
			LOD 200
			ZTest Greater

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows vertex:vert alphatest:_Cutoff addshadow 

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 5.0
			#include "noiseSimplex.cginc"
			#define M_PI 3.14159265359
			sampler2D _MainTex;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct Input
			{
				float2 uv_MainTex;
				float3 vertexPos;
				float Time;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;


			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.

			void vert(inout appdata v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.vertexPos = v.vertex.xyz;
				o.uv_MainTex = v.texcoord;
				o.Time = _Time;
			}

			float _WaveColorAmplifier;
			float _Uvspeed;
			float _UvMoveDist;
			float _UvFrequency;
			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				float3 sPos = float3(IN.uv_MainTex.x, IN.uv_MainTex.y, 0) *_UvFrequency;
				sPos.z += _Time.x * _Uvspeed;
				float noise = _UvMoveDist * ((snoise(sPos) + 1) / 2);
				float4 noiseToDirection = float4(cos(noise * M_PI * 2), sin(noise * M_PI * 2), 0, 0);

				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Alpha = 1 - tex2D(_MainTex, IN.uv_MainTex + normalize(noiseToDirection) + (_Time * 0.02f)).r;
			}
			ENDCG

				//pass
				//{
				//	Name "Water"
				//	Cull Off
				//	ZTest LEqual

				//	CGPROGRAM
				//	#pragma vertex vert
				//	#pragma fragment frag
				//	#include "UnityCG.cginc"
				//	#include "noiseSimplex.cginc"
				//	#include "Lighting.cginc"
				//	#include "AutoLight.cginc"
				//	#define M_PI 3.14159265359

				//	struct v2f
				//	{
				//		float4 vertexPos    : SV_POSITION;
				//		float2 uv           : TEXCOORD0;
				//		float3 localPos 	: TEXCOORD1;
				//		float3 normal		: NORMAL;
				//	};

				//	float _NoiseAmplifier;
				//	float _NoiseSpeed;
				//	float _NoiseFrequency;
				//	v2f vert(appdata_full v)
				//	{
				//		v2f o;
				//		o.localPos = v.vertex.xyz;

				//		v.vertex = UnityObjectToClipPos(v.vertex);
				//		v.vertex.y += snoise((float3(o.localPos.xyz) + (_Time * _NoiseSpeed)) * _NoiseFrequency) * _NoiseAmplifier;
				//		o.localPos += snoise((float3(o.localPos.xyz) + (_Time * _NoiseSpeed)) * _NoiseFrequency) * _NoiseAmplifier;

				//		o.uv = v.texcoord.xy;
				//		o.vertexPos = v.vertex;
				//		o.normal = v.normal;
				//		return o;
				//	}

				//	fixed4 _Color;
				//	sampler2D _MainTex;
				//	float4 _MainTex_ST;
				//	float _WaveColorAmplifier;

				//	float _Uvspeed;
				//	float _UvMoveDist;
				//	float _UvFrequency;
				//	float _Cutoff;
				//	half4 frag(v2f i) : COLOR
				//	{
				//		float3 sPos = float3(i.uv.x, i.uv.y, 0) *_UvFrequency;
				//		sPos.z += _Time.x * _Uvspeed;
				//		float noise = _UvMoveDist * ((snoise(sPos) + 1) / 2);
				//		float4 noiseToDirection = float4(cos(noise * M_PI * 2), sin(noise * M_PI * 2), 0, 0);

				//		// Albedo comes from a texture tinted by color
				//		float2 newUV = TRANSFORM_TEX(i.uv, _MainTex);

				//		fixed4 c = tex2D(_MainTex, newUV + normalize(noiseToDirection) + (_Time * 0.02f)) *_Color;

				//		float colChange = i.localPos.y * -_WaveColorAmplifier;
				//		float4 col = float4(c.rgb + float3(colChange, colChange, colChange), 0.2f);

				//		float diff = max(dot(i.normal, _WorldSpaceLightPos0), 0.0);
				//		return col * diff;
				//	}
				//	ENDCG
				//}
		}
			FallBack "Diffuse"
}