// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Copyright (C) 2014 Stephan Schaem - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

Shader "TMPro/Bitmap" {

Properties {
	_MainTex		("Font Atlas", 2D) = "white" {}
	_FaceTex		("Font Texture", 2D) = "white" {}
	_FaceColor		("Text Color", Color) = (1,1,1,1)

	_VertexOffsetX	("Vertex OffsetX", float) = 0
	_VertexOffsetY	("Vertex OffsetY", float) = 0
	_MaskCoord		("Mask Coords", vector) = (0,0,100000,100000)
	_MaskSoftnessX	("Mask SoftnessX", float) = 0
	_MaskSoftnessY	("Mask SoftnessY", float) = 0
}

SubShader {

	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Lighting Off
	Cull [_CullMode]
	Ztest [_ZTestMode]
	ZWrite Off 
	Fog { Mode Off }
	Blend SrcAlpha OneMinusSrcAlpha

	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma multi_compile MASK_OFF MASK_HARD MASK_SOFT

		#include "UnityCG.cginc"

		struct appdata_t {
			float4 vertex		: POSITION;
			fixed4 color		: COLOR;
			float2 texcoord0	: TEXCOORD0;
			float2 texcoord1	: TEXCOORD1;
		};

		struct v2f {
			float4	vertex		: POSITION;
			fixed4	color		: COLOR;
			float2	texcoord0	: TEXCOORD0;
			float2	texcoord1	: TEXCOORD1;
		#if MASK_HARD | MASK_SOFT
			float4	mask		: TEXCOORD2;
		#endif
		};

		uniform	sampler2D 	_MainTex;
		uniform	sampler2D 	_FaceTex;
		uniform	fixed4		_FaceColor;

		uniform float		_VertexOffsetX;
		uniform float		_VertexOffsetY;
		uniform float4		_MaskCoord;
		uniform float		_MaskSoftnessX;
		uniform float		_MaskSoftnessY;

		float2 UnpackUV(float uv) { return float2(floor(uv) * 4.0 / 4096.0, frac(uv) * 4.0); }

		v2f vert (appdata_t i)
		{
			float4 vert = i.vertex;
			vert.x += _VertexOffsetX;
			vert.y += _VertexOffsetY;
			float4 vPosition = UnityPixelSnap(UnityObjectToClipPos(vert));

			fixed4 faceColor = i.color;
			if(faceColor.a > .5) faceColor.a -= .5;
			faceColor.a *= 2;
			faceColor *= _FaceColor;

			v2f o;
			o.vertex = vPosition;
			o.color = faceColor;
			o.texcoord0 = i.texcoord0;
			o.texcoord1 = UnpackUV(i.texcoord1);
		#if MASK_HARD | MASK_SOFT
			float2 pixelSize = vPosition.w;
			pixelSize /= float2(_ScreenParams.x * UNITY_MATRIX_P[0][0], _ScreenParams.y * UNITY_MATRIX_P[1][1]);
		    //pixelSize /= float2(_ScaleX, _ScaleY) * mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy);
			o.mask = float4(vert.xy-_MaskCoord.xy, .5/pixelSize.xy);
		#endif
			return o;
		}

		fixed4 frag (v2f i) : COLOR
		{
			fixed4 c = tex2D(_FaceTex, i.texcoord1) * i.color;
			c.a *= tex2D(_MainTex, i.texcoord0).a;

		#if MASK_HARD
			float2 m = 1-saturate((abs(i.mask.xy)-_MaskCoord.zw)*i.mask.zw);
			c.a *= m.x*m.y;
		#endif

		#if MASK_SOFT
			float2 s = half2(_MaskSoftnessX, _MaskSoftnessY)*i.mask.zw;
			float2 m = 1-saturate(((abs(i.mask.xy)-_MaskCoord.zw)*i.mask.zw+s) / (1+s));
			m *= m;
			c.a *= m.x*m.y;
		#endif

			return c;
		}
		ENDCG
	}
}

	//CustomEditor "TMPro_SDFMaterialEditor"
}
