Shader "Custom/TestTransparencyShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}
		SubShader
		{
			Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
			LOD 200
			Cull Off
			Zwrite Off

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows vertex:vert alphatest:_Cutoff addshadow

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
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

			float colChange = IN.vertexPos.y * _WaveColorAmplifier;
			//o.Albedo = c.rgb + float3(colChange, colChange, colChange);
			o.Albedo = float3(IN.vertexPos.x, IN.vertexPos.z, IN.vertexPos.y);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = tex2D(_MainTex, IN.uv_MainTex + float2(_Time.b * 0.1f, _Time.g * 0.1f)).r;// (sin(IN.vertexPos.x) + 1) * 0.5f;
		}
		ENDCG
		}
			FallBack "Diffuse"
}