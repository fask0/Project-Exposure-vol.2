Shader "Custom/ImprovedFishShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_MetallicTex("Metallic Tex", 2D) = "white" {}
		_NormalMap("Normal Map", 2D) = "white" {}
		_HeightMap("Height Map", 2D) = "white" {}
		_Occlusion("Occlusion", 2D) = "white" {}

		_WobbleSpeed("WobbleSpeed", Range(0, 1000)) = 3
		_WobbleDistance("WobbleDistance", Range(0, 1)) = 1
		_WobbleCurve("WobbleCurve", Range(0, 1000)) = 5

		_Offset("Offset", Range(0, 1)) = 0
		_FishLength("FishLength", Range(0, 10)) = 1
		_Specular("Specular", Range(0, 1)) = 0
	}

		SubShader
		{
			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows addshadow vertex:vert

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input
			{
				float2 uv_MainTex;
				float3 vertexPos;
				float3 worldRefl;
			};

			float _WobbleSpeed;
			float _WobbleDistance;
			float _WobbleCurve;

			float _FishLength;
			float _Offset;
			sampler2D _NoiseTex;
			sampler2D _NormalMap;
			sampler2D _HeightMap;
			void vert(inout appdata_full v, out Input o)
			{
				//Move uvs
				v.vertex.x += ((sin((_Time + _Offset) * _WobbleSpeed + (v.vertex.z * 2 + _FishLength) * (v.vertex.z * 2 + _FishLength) * _WobbleCurve)) * (v.vertex.z * 2 + _FishLength)) * _WobbleDistance;

				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.vertexPos = v.vertex;// +v.normal * UnpackNormal(tex2Dlod(_HeightMap, float4(v.texcoord.xy, 0, 0)));
			}

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			float _Specular;

			sampler2D _MetallicTex;
			sampler2D _Occlusion;
			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;

				// Metallic and smoothness come from slider variables
				o.Metallic = tex2D(_MetallicTex, IN.uv_MainTex) + _Metallic; //_Metallic;
				o.Normal = UnpackNormal(tex2Dlod(_NormalMap, float4(IN.uv_MainTex.xy, 0, 0)));
				o.Occlusion = tex2D(_Occlusion, IN.uv_MainTex);
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
