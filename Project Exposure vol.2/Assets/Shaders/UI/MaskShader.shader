Shader "Custom/MaskShader"
{
	SubShader
	{
		Tags { "RenderType" = "Geometry-1" }
		ColorMask 0
		ZWrite Off

		Pass
		{
			Stencil
			{
				Ref 1
				Comp Always
				Pass Replace
			}
		}
	}
}
