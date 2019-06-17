Shader "Custom/WaterShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_NoiseSpeed("NoiseSpeed", Range(0, 100)) = 1
		_NoiseAmplifier("NoiseAmplifier", Range(0, 10)) = 0.2
		_NoiseFrequency("NoiseFrequency", Range(0, 10)) = 0.1

		_WaveDirection("WaveDirection", Vector) = (0,0,0,0)
		_WaveAmplitude("WaveAmplitude", Range(0, 10)) = 1
		_WaveLength("WaveLength", Range(0, 10)) = 3
		_WaveSpeed("WaveSpeed", Range(0, 100)) = 30
		_WaveSteepness("WaveSteepness", Range(0, 50)) = 0.8
		_WaveColorAmplifier("WaveColorAmplifier", Range(0, 1)) = 0.2
	}
		SubShader
		{
			//Tags { "RenderType" = "Opaque" }
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			LOD 200
			Cull Off
			Zwrite On
			//ZTest Greater

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows addshadow vertex:vert alpha:fade

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 5.0
			#include "noiseSimplex.cginc"
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
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.

			float4 _WaveDirection;
			float _WaveAmplitude;
			float _WaveLength;
			float _WaveSpeed;
			float _WaveSteepness;
			float _NoiseAmplifier;
			float _NoiseSpeed;
			float _NoiseFrequency;
			void vert(inout appdata v, out Input o)
			{
					UNITY_INITIALIZE_OUTPUT(Input, o);
					o.vertexPos = v.vertex * v.normal;
				}

				float _WaveColorAmplifier;
				void surf(Input IN, inout SurfaceOutputStandard o)
				{
					// Albedo comes from a texture tinted by color
					fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

					float colChange = IN.vertexPos.y * _WaveColorAmplifier;
					o.Albedo = c.rgb + float3(colChange, colChange, colChange);
					// Metallic and smoothness come from slider variables
					o.Metallic = _Metallic;
					o.Smoothness = _Glossiness;
					o.Alpha = c.a;
				}
				ENDCG
		}
			FallBack "Diffuse"
}