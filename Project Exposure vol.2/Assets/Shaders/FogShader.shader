Shader "Hidden/FogShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_FogColor("Fog Color", Color) = (1,1,1,1)
		_SilhouetteColor("Silhouette Color", Color) = (1,1,1,1)
		_DepthStart("Depth Start", float) = 1
		_DepthDistance("Depth Distance", float) = 1
		_FadeDistance("Depth Fade Distance", Range(0, 1)) = 0.5
		_FogBeforeFadeMultiplier("Fog Before Fade", Range(0, 1)) = 0.5
		_FogStrength("Fog Strength", Range(0.1, 1)) = 1
		_SingleColor("Single Color", Range(0, 1)) = 0
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

				sampler2D _CameraDepthTexture;
				fixed4 _FogColor;
				fixed4 _SilhouetteColor;
				float _DepthStart;
				float _DepthDistance;
				float _FadeDistance;
				float _FogBeforeFadeMultiplier;

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
				float _FogStrength;
				float _SingleColor;
				fixed4 frag(v2f i) : COLOR
				{
						float depthValue = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r)* _ProjectionParams.z;
						float depthValue2 = saturate((depthValue - _DepthDistance) / _DepthDistance * _FadeDistance);
						depthValue = saturate((depthValue - _DepthStart) / _DepthDistance);
						fixed4 fogColor = _FogColor * depthValue;
						fixed4 fogColor2 = lerp(_FogColor, _SilhouetteColor, 1 - depthValue2);
						fixed4 col = tex2Dproj(_MainTex, i.scrPos);

						fixed4 newCol = lerp(col, fogColor, depthValue * _FogBeforeFadeMultiplier);
						return lerp(col, lerp(newCol, fogColor2, depthValue), _FogStrength);
				}
				ENDCG
		}
		}
}
