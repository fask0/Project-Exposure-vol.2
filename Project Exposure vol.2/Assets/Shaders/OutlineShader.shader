Shader "Custom/OutlineShader"
{
	Properties
	{
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
		_OutlineWidth("Outline Width", Range(0.0, 2.0)) = 1.05
	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" }
		LOD 200
		//ZTest NotEqual

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		struct Input
		{
			float4 vertex : POSITION;
			float2 uv_MainTex;
		};

		float _OutlineWidth;
		void vert(inout appdata_full v, out Input o)
		{
			o.uv_MainTex = v.texcoord;

			//v.vertex += float4(v.normal * _OutlineWidth * 0.1f, 1);

			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.vertex = v.vertex;
		}

		fixed4 _OutlineColor;
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			// Albedo comes from a texture tinted by color
			//fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			//o.Albedo = c.rgb;
			//// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			//o.Alpha = c.a;
			o.Emission = _OutlineColor;
			o.Albedo = _OutlineColor;
			if (_OutlineWidth > 0)
			{
				o.Alpha = _OutlineColor.a;
			}
			else
			{
				o.Alpha = 0;
			}
		}
		ENDCG
	}
		FallBack "Diffuse"
}
