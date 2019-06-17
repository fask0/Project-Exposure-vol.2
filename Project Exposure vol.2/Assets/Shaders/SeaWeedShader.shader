Shader "Custom/SeaWeedShader"
{
	Properties
	{
		_MinDist("Min Tessellation Distance", float) = 10
		_MaxDist("Max Tessellation Distance", float) = 50
		_Tess("Tessellation", Range(1, 20)) = 4
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_NormalMap("Normal Map", 2D) = "bump" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_PlayerDetectionRange("Player Detection Range", Range(0, 10)) = 1.5
		_CurveHeight("Curve Height", Range(0, 10)) = 3
		_CurveWidth("Curve Width", Range(0, 3)) = 1.5

		_WobbleSpeed("Wobble Speed", Range(0, 1)) = 0.5
		_WobbleDistance("Wobble Distance", Range(0, 10)) = 0.5
		_WobbleCurve("Wobble Curve", Range(0, 10)) = 0.5

		_LeanDirection("Lean Direction", Vector) = (1, 0,0,0)
		_LeanDistance("Lean Distance", Range(0, 10)) = 4
		_Offset("Offset", float) = 0

		_HighestY("HighestY", float) = 1
		_LowestY("LowestY", float) = 0

		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_PlayerPos("Player position", Vector) = (0,0,0)
	}
		SubShader
		{
			Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TreeTransparentCutout" }
			Cull Off
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows vertex:vert tessellate:tess alphatest:_Cutoff addshadow

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 5.0
			#include "noiseSimplex.cginc"
			#include "Tessellation.cginc"

			sampler2D _MainTex;
			sampler2D _NormalMap;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 texcoord : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
			};

			struct Input
			{
				float2 uv_MainTex;
				float3 vertexPos;
			};


			float _MinDist;
			float _MaxDist;
			float _Tess;
			float4 tess(appdata v0, appdata v1, appdata v2)
			{
				return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, _MinDist, _MaxDist, _Tess);
			}

			float _WobbleSpeed;
			float _WobbleDistance;
			float _WobbleCurve;

			float _PlayerDetectionRange;
			float _CurveHeight;
			float _CurveWidth;

			float2 _LeanDirection;
			float _LeanDistance;
			float _HighestY;
			float _LowestY;
			float _Offset;
			float3 _PlayerPos;
			sampler2D _NoiseTex;
			void vert(inout appdata v)
			{
				//Vertex pos to world pos shite
				float4 wPos = mul(unity_ObjectToWorld, v.vertex);
				float diff = _HighestY - _LowestY;
				float relativeYPos = ((wPos.y - _LowestY) / diff);

				float4 v0 = wPos;
				float4 v1 = mul(unity_ObjectToWorld, float4(0, v.vertex.y, 0, v.vertex.w));
				if (_Offset > 0)
				{
					//The lean bois
					v0.x += (relativeYPos * relativeYPos * _LeanDistance) * _LeanDirection.x;
					v0.z += (relativeYPos * relativeYPos * _LeanDistance) * _LeanDirection.y;
					v1.x += (relativeYPos * relativeYPos * _LeanDistance) * _LeanDirection.x;
					v1.z += (relativeYPos * relativeYPos * _LeanDistance) * _LeanDirection.y;

					//The wobble wobble wobble
					v0.x += ((snoise((_Time + _Offset) * _WobbleSpeed + relativeYPos * relativeYPos * _WobbleCurve)) * relativeYPos) * _WobbleDistance;
					v0.z += ((snoise((_Time + _Offset / 2) * _WobbleSpeed + relativeYPos * relativeYPos * _WobbleCurve) + 0.5) * relativeYPos) * _WobbleDistance;
					v1.x += ((snoise((_Time + _Offset) * _WobbleSpeed + relativeYPos * relativeYPos * _WobbleCurve)) * relativeYPos) * _WobbleDistance;
					v1.z += ((snoise((_Time + _Offset / 2) * _WobbleSpeed + relativeYPos * relativeYPos * _WobbleCurve) + 0.5) * relativeYPos) * _WobbleDistance;
				}

				//Bob n weave player
				float4 middleObjPos = mul(unity_WorldToObject, v1);
				float3 weedCenter = mul(unity_ObjectToWorld, float4(middleObjPos.x, 0, middleObjPos.z, 1));
				float3 diff2 = weedCenter - float3(_PlayerPos.x, weedCenter.y, _PlayerPos.z);
				float lengthVal = length(diff2);
				float2 direction = float2(normalize(diff2).x, normalize(diff2).z);

				float diffY = _CurveHeight - abs(max(min(_PlayerPos.y - v0.y, _CurveHeight), -_CurveHeight));
				if (lengthVal < _PlayerDetectionRange)
				{
					direction = float2(-direction.x, -direction.y);
					direction = direction * (_PlayerDetectionRange - lengthVal) * _CurveWidth;

					v0.x += -direction.x * relativeYPos * diffY;
					v0.z += -direction.y * relativeYPos * diffY;
				}


				//Return new pos
				float4 objPos = mul(unity_WorldToObject, v0);
				v.vertex.xyz = objPos;
			}

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
				o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
			}
			ENDCG
		}
			FallBack "Nature/Soft"
}
