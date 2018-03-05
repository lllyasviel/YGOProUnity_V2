// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Copyright (C) 2014 Stephan Schaem - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreementoutline
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

Shader "TMPro/Distance Field" {

Properties {
	_FaceTex			("Face Texture", 2D) = "white" {}
	_FaceUVSpeedX		("Face UV Speed X", Range(-5, 5)) = 0.0
	_FaceUVSpeedY		("Face UV Speed Y", Range(-5, 5)) = 0.0
	_FaceColor			("Face Color", Color) = (1,1,1,1)
	_FaceDilate			("Face Dilate", Range(-1,1)) = 0

	_OutlineColor		("Outline Color", Color) = (0,0,0,1)
	_OutlineTex			("Outline Texture", 2D) = "white" {}
	_OutlineUVSpeedX	("Outline UV Speed X", Range(-5, 5)) = 0.0
	_OutlineUVSpeedY	("Outline UV Speed Y", Range(-5, 5)) = 0.0
	_OutlineWidth		("Outline Thickness", Range(0, 1)) = 0
	_OutlineSoftness	("Outline Softness", Range(0,1)) = 0

	_Bevel				("Bevel", Range(0,1)) = 0.5
	_BevelOffset		("Bevel Offset", Range(-0.5,0.5)) = 0
	_BevelWidth			("Bevel Width", Range(-.5,0.5)) = 0
	_BevelClamp			("Bevel Clamp", Range(0,1)) = 0
	_BevelRoundness		("Bevel Roundness", Range(0,1)) = 0

	_LightAngle			("Light Angle", Range(0.0, 6.2831853)) = 3.1416
	_SpecularColor		("Specular", Color) = (1,1,1,1)
	_SpecularPower		("Specular", Range(0,4)) = 2.0
	_Reflectivity		("Reflectivity", Range(5.0,15.0)) = 10
	_Diffuse			("Diffuse", Range(0,1)) = 0.5
	_Ambient			("Ambient", Range(1,0)) = 0.5

	_BumpMap 			("Normal map", 2D) = "bump" {}
	_BumpOutline		("Bump Outline", Range(0,1)) = 0
	_BumpFace			("Bump Face", Range(0,1)) = 0

	_ReflectFaceColor		("Reflection Color", Color) = (0,0,0,1)
	_ReflectOutlineColor	("Reflection Color", Color) = (0,0,0,1)
	_Cube 					("Reflection Cubemap", Cube) = "black" { /* TexGen CubeReflect */ }
	_EnvMatrixRotation		("Texture Rotation", vector) = (0, 0, 0, 0)
		

	_UnderlayColor		("Border Color", Color) = (0,0,0, 0.5)
	_UnderlayOffsetX	("Border OffsetX", Range(-1,1)) = 0
	_UnderlayOffsetY	("Border OffsetY", Range(-1,1)) = 0
	_UnderlayDilate		("Border Dilate", Range(-1,1)) = 0
	_UnderlaySoftness	("Border Softness", Range(0,1)) = 0

	_GlowColor			("Color", Color) = (0, 1, 0, 0.5)
	_GlowOffset			("Offset", Range(-1,1)) = 0
	_GlowInner			("Inner", Range(0,1)) = 0.05
	_GlowOuter			("Outer", Range(0,1)) = 0.05
	_GlowPower			("Falloff", Range(1, 0)) = 0.75

	_WeightNormal		("Weight Normal", float) = 0
	_WeightBold			("Weight Bold", float) = 0.5

	_ShaderFlags		("Flags", float) = 0
	_ScaleRatioA		("Scale RatioA", float) = 1
	_ScaleRatioB		("Scale RatioB", float) = 1
	_ScaleRatioC		("Scale RatioC", float) = 1

	_MainTex			("Font Atlas", 2D) = "white" {}
	_TextureWidth		("Texture Width", float) = 512
	_TextureHeight		("Texture Height", float) = 512
	_GradientScale		("Gradient Scale", float) = 5.0
	_ScaleX				("Scale X", float) = 1.0
	_ScaleY				("Scale Y", float) = 1.0
	_PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875

	_VertexOffsetX		("Vertex OffsetX", float) = 0
	_VertexOffsetY		("Vertex OffsetY", float) = 0
	_MaskID				("Mask ID", float) = 0
	_MaskCoord			("Mask Coords", vector) = (0,0,0,0)
	_MaskSoftnessX		("Mask SoftnessX", float) = 0
	_MaskSoftnessY		("Mask SoftnessY", float) = 0

	_StencilComp ("Stencil Comparison", Float) = 8
	_Stencil ("Stencil ID", Float) = 0
	_StencilOp ("Stencil Operation", Float) = 0
	_StencilWriteMask ("Stencil Write Mask", Float) = 255
	_StencilReadMask ("Stencil Read Mask", Float) = 255

	//_ColorMask ("Color Mask", Float) = 15	
}

SubShader {

	Tags
	{ 
		"Queue"="Transparent"
		"IgnoreProjector"="True"
		"RenderType"="Transparent"
	}

	Stencil
	{
		Ref [_Stencil]
		Comp [_StencilComp]
		Pass [_StencilOp] 
		ReadMask [_StencilReadMask]
		WriteMask [_StencilWriteMask]
	}

	Cull [_CullMode]
	ZWrite Off
	Lighting Off
	Fog { Mode Off }
	Ztest [_ZTestMode]
	Blend One OneMinusSrcAlpha
	//ColorMask [_ColorMask]	

	Pass {
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex VertShader
		#pragma fragment PixShader
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma multi_compile BEVEL_OFF BEVEL_ON
		#pragma multi_compile UNDERLAY_OFF UNDERLAY_ON UNDERLAY_INNER
		#pragma multi_compile GLOW_OFF GLOW_ON
		#pragma multi_compile MASK_OFF MASK_HARD MASK_SOFT
		#pragma glsl

		#include "UnityCG.cginc"
		#include "TMPro_Properties.cginc"
		#include "TMPro.cginc"

		struct vertex_t {
			float4	vertex			: POSITION;
			float3	normal			: NORMAL;
			fixed4	color			: COLOR;
			float2	texcoord0		: TEXCOORD0;
			float2	texcoord1		: TEXCOORD1;
		};

		struct pixel_t {
			float4	vertex			: SV_POSITION;
			fixed4	color			: COLOR;
			fixed4	faceColor		: COLOR1;
			fixed4	outlineColor	: COLOR2;
			float4	texcoords		: TEXCOORD0;		// Atlas & Texture
			float4	param			: TEXCOORD1;		// alphaClip, scale, bias, weight
			float4	mask			: TEXCOORD2;		// Position in object space(xy), pixel Size(zw)
			float3	viewDir			: TEXCOORD3;
		#if (UNDERLAY_ON || UNDERLAY_INNER)
			float4	texcoord2		: TEXCOORD4;		// u,v, scale, bias
			fixed4	underlayColor	: TEXCOORD5;
		#endif
		};

		pixel_t VertShader(vertex_t input)
		{
			float bold = step(input.texcoord1.y, 0);

			float4 vert = input.vertex;
			vert.x += _VertexOffsetX;
			vert.y += _VertexOffsetY;
			float4 vPosition = UnityObjectToClipPos(vert);

			float2 pixelSize = vPosition.w;
			pixelSize /= float2(_ScaleX, _ScaleY) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
			float scale = rsqrt(dot(pixelSize, pixelSize));
			scale *= abs(input.texcoord1.y) * _GradientScale * 1.5;
			if(UNITY_MATRIX_P[3][3] == 0) scale = lerp(scale*(1 - _PerspectiveFilter), scale, abs(dot(UnityObjectToWorldNormal(input.normal.xyz), normalize(ObjSpaceViewDir(vert)))));

			float weight = (lerp(_WeightNormal, _WeightBold, bold)) / _GradientScale;
			weight += _FaceDilate * _ScaleRatioA * 0.5;

			float bias =(.5 - weight) + (.5 / scale);

			float alphaClip = (1.0 - _OutlineWidth*_ScaleRatioA - _OutlineSoftness*_ScaleRatioA);
		#if GLOW_ON
			alphaClip = min(alphaClip, 1.0 - _GlowOffset * _ScaleRatioB - _GlowOuter * _ScaleRatioB);
		#endif

			alphaClip = alphaClip / 2.0 - ( .5 / scale) - weight;

			float opacity = input.color.a;
			float4 faceColor =_FaceColor;
			faceColor.rgb *= input.color.rgb;
			faceColor.a *= opacity;

			float4 outlineColor =_OutlineColor;
			outlineColor.a *= opacity;

		#if (UNDERLAY_ON || UNDERLAY_INNER)
			float4 underlayColor = _UnderlayColor;
			underlayColor.a *= opacity;
			underlayColor.rgb *= underlayColor.a;

			float bScale = scale;
			bScale /= 1+((_UnderlaySoftness*_ScaleRatioC)*bScale);
			float bBias = (.5-weight)*bScale - .5 - ((_UnderlayDilate*_ScaleRatioC)*.5*bScale);

			float x = -(_UnderlayOffsetX * _ScaleRatioC) * _GradientScale / _TextureWidth;
			float y = -(_UnderlayOffsetY * _ScaleRatioC) * _GradientScale / _TextureHeight;
			float2 bOffset = float2(x, y);
		#endif

			pixel_t output = {
				vPosition,
				input.color, faceColor, outlineColor,
				float4(input.texcoord0, UnpackUV(input.texcoord1.x)),
				float4(alphaClip, scale, bias, weight),
				float4(vert.xy-_MaskCoord.xy, .5/pixelSize.xy),
				mul((float3x3)_EnvMatrix, _WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, vert).xyz),
			#if (UNDERLAY_ON || UNDERLAY_INNER)
				float4(input.texcoord0 + bOffset, bScale, bBias),
        underlayColor,
			#endif
			};

			return output;
		}

		fixed4 PixShader(pixel_t input) : COLOR
		{
			float c = tex2D(_MainTex, input.texcoords.xy).a;
		#ifndef UNDERLAY_ON
			clip(c - input.param.x);
		#endif

			float	scale	= input.param.y;
			float	bias	= input.param.z;
			float	weight	= input.param.w;

			float sd = (bias - c) * scale;

			float outline = (_OutlineWidth*_ScaleRatioA) * scale;
			float softness = (_OutlineSoftness*_ScaleRatioA) * scale;

			half4 faceColor = input.faceColor;
			half4 outlineColor = input.outlineColor;
			
			faceColor *= tex2D(_FaceTex, float2(input.texcoords.z + _FaceUVSpeedX * _Time.y, input.texcoords.w + _FaceUVSpeedY * _Time.y));
			outlineColor *= tex2D(_OutlineTex, float2(input.texcoords.z + _OutlineUVSpeedX * _Time.y, input.texcoords.w + _OutlineUVSpeedY * _Time.y));

			faceColor = GetColor(sd, faceColor, outlineColor, outline, softness);

		#if BEVEL_ON
			float3 dxy = float3(.5/_TextureWidth, .5/_TextureHeight, 0);
			float3 n = GetSurfaceNormal(input.texcoords.xy, weight, dxy);

			float3 bump = UnpackNormal(tex2D(_BumpMap, input.texcoords.zw)).xyz;
			bump *= lerp(_BumpFace, _BumpOutline, saturate(sd + outline * 0.5));
			n = normalize(n- bump);

			float3 light = normalize(float3(sin(_LightAngle), cos(_LightAngle), -1.0));

			float3 col = GetSpecular(n, light);
			faceColor.rgb += col*faceColor.a;
			faceColor.rgb *= 1-(dot(n, light)*_Diffuse);
			faceColor.rgb *= lerp(_Ambient, 1, n.z*n.z);

			fixed4 reflcol = texCUBE(_Cube, reflect(input.viewDir, -n));
			faceColor.rgb += reflcol.rgb * lerp(_ReflectFaceColor.rgb, _ReflectOutlineColor.rgb, saturate(sd + outline * 0.5)) * faceColor.a;
			//faceColor.rgb += reflcol.rgb * _ReflectFaceColor.rgb * faceColor.a;
		#endif

		#if UNDERLAY_ON
			float d = tex2D(_MainTex, input.texcoord2.xy).a * input.texcoord2.z;
			faceColor += input.underlayColor * saturate(d - input.texcoord2.w) * (1-faceColor.a);
		#endif

		#if UNDERLAY_INNER
			float d = tex2D(_MainTex, input.texcoord2.xy).a * input.texcoord2.z;
			faceColor += input.underlayColor * (1-saturate(d - input.texcoord2.w)) * saturate(1-sd) * (1-faceColor.a);
		#endif

		#if GLOW_ON
			float4 glowColor = GetGlowColor(sd, scale);
			faceColor.rgb += glowColor.rgb * glowColor.a * input.color.a;
			//faceColor.a *= glowColor.a; // Required for Alpha when using Render Textures
		#endif

		#if MASK_HARD
			float2 m = 1-saturate((abs(input.mask.xy)-_MaskCoord.zw)*input.mask.zw);
			faceColor *= m.x*m.y;
		#endif

		#if MASK_SOFT
			float2 s = half2(_MaskSoftnessX, _MaskSoftnessY)*input.mask.zw;
			float2 m = 1-saturate(((abs(input.mask.xy)-_MaskCoord.zw)*input.mask.zw+s) / (1+s));
			m *= m;
			faceColor *= m.x*m.y;
		#endif

			return faceColor;
		}
		ENDCG
	}
}


Fallback "TMPro/Mobile/Distance Field"
CustomEditor "TMPro_SDFMaterialEditor"
}
