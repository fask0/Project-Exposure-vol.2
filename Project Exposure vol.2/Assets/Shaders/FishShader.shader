Shader "Custom/FishShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_ScanLineColor("ScanLineColor", color) = (1,1,1,1)
		_ScanInbetweenColor("ScanInbetweenColor", Color) = (1,1,1,1)
		_ScanTex("Scan (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_WobbleSpeed("WobbleSpeed", Range(0, 1000)) = 0
		_WobbleDistance("WobbleDistance", Range(0, 1)) = 0
		_WobbleCurve("WobbleCurve", Range(0, 1000)) = 0

		_Offset("Offset", Range(0, 1)) = 0
		_FishLength("FishLength", Range(0, 10)) = 0
		_Specular("Specular", Range(0, 1)) = 0

		_IsScanning("IsScanning", Range(0,1)) = 0
		_ScanLines("ScanLines", Range(0, 100)) = 1
		_ScanLineWidth("ScanLineWidth", Range(0, 2)) = 2
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows addshadow vertex:vert

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0
#include "noiseSimplex.cginc"
			sampler2D _MainTex;
			sampler2D _ScanTex;

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
			float _IsScanning;
			float4 _ScanTex_ST;
			sampler2D _NoiseTex;
			void vert(inout appdata_full v, out Input o)
			{
				//Move uvs
				v.vertex.z += ((sin((_Time + _Offset) * _WobbleSpeed + (-v.vertex.x + _FishLength) * (-v.vertex.x + _FishLength) * _WobbleCurve)) * (-v.vertex.x + _FishLength)) * _WobbleDistance;

				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.vertexPos = v.vertex;
			}

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			fixed4 _ScanLineColor;
			fixed4 _ScanInbetweenColor;
			float _ScanLines;
			float _ScanLineWidth;
			float _Specular;
			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				if(_IsScanning == 1)
				{
					for (int i = 0; i < _ScanLines; ++i)
					{
						if (IN.uv_MainTex.y > (_ScanTex_ST.y / _ScanLines) * i && IN.uv_MainTex.y < (_ScanTex_ST.y / _ScanLines) * i + _ScanLineWidth)
						{
							//							0.4 		 sub 1/3 (0.333) 0.4 - 0.33 = 0.07/_scanLineWidth
							float perc = ((IN.uv_MainTex.y - (_ScanTex_ST.y / _ScanLines) * i) / _ScanLineWidth);
							c = lerp(c + _ScanLineColor * 0.025f, _ScanLineColor, (perc * perc));
							if (perc > 0.9f)
								o.Emission = (perc - 0.9f) * 4;

							o.Emission += ((snoise((IN.uv_MainTex * float2(20, 100)) + (_Time * 10.0f)) + 1) * 0.5f) * (perc * perc * perc * perc * perc * perc);
						}
						else
						{
							c += _ScanInbetweenColor * 0.025f;
						}
					}
				}
				// Albedo comes from a texture tinted by color
				o.Albedo = c.rgb;

				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
