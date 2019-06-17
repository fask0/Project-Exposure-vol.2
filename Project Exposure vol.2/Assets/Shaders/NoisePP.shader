Shader "Hidden/UnderwaterImageEffect"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}

		_NoiseScale("Noise Scale", float) = 1
		_NoiseFrequency("Noise Frequency", float) = 1
		_NoiseSpeed("Noise Speed", float) = 1
		_PixelOffset("Pixel Offset", float) = 0.005

		_DepthStart("Depth Start", float) = 1
		_DepthDistance("Depth Distance", float) = 1
	}
		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"
				#include "noiseSimplex.cginc"
				#define M_PI 3.14159265359

				uniform float _NoiseScale;
				uniform float _NoiseFrequency;
				uniform float _NoiseSpeed;
				uniform float _PixelOffset;
				float _DepthStart;
				float _DepthDistance;
				sampler2D _CameraDepthTexture;

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
					float4 scrPos : TEXCOORD1;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.scrPos = ComputeScreenPos(o.vertex);
					return o;
				}

				sampler2D _MainTex;

				fixed4 frag(v2f i) : COLOR
				{
					//Depth
					float depthValue = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r)* _ProjectionParams.z;
					depthValue = saturate((depthValue - _DepthStart) / _DepthDistance);

					//Wiggle wiggle wiggle
					float3 sPos = float3(i.scrPos.x, i.scrPos.y, 0) * _NoiseFrequency;
					sPos.z += _Time.x * _NoiseSpeed;
					float noise = _NoiseScale * ((snoise(sPos) + 1) / 2);
					float4 noiseToDirection = float4(cos(noise * M_PI * 2), sin(noise * M_PI * 2), 0, 0);
					fixed4 color = tex2Dproj(_MainTex, i.scrPos + (normalize(noiseToDirection) * _PixelOffset * (1 - depthValue)));
					return color;
				}
				ENDCG
		}
		}
}
