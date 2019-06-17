Shader "Unlit/UndiscoveredMinimapShader"
{
    Properties
    {
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _MainTexWidth ("Texture Width", float) = 0
        _MainTexHeight ("Texture Height", float) = 0
        _PlayerPositionX ("PlayerX", float) = 0
        _PlayerPositionY ("PlayerY", float) = 0
        _Radius ("Radius", float) = 0
        _RadiusSquared ("Radius Squared", float) = 0

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _DummyTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _MainTexWidth;
            float _MainTexHeight;
            float _PlayerPositionX;
            float _PlayerPositionY;
            float _Radius;
            float _RadiusSquared;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 distance = float2(i.uv.x * _MainTexWidth, i.uv.y * _MainTexHeight) - float2(_PlayerPositionX, _PlayerPositionY);
                float magnitude = distance.x * distance.x + distance.y * distance.y;
                if (magnitude <= _RadiusSquared)
                {
                    col.a = 0;
                }

                return col;
            }
            ENDCG
        }
    }
}
