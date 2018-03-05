﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Effects/Water" {
Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
        _Shininess ("Shininess", Range (0.01, 10)) = 0.078125
        _ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
		_RimColor("Rim Color", Color) = (1,1,1,0.5)
        _BumpMap ("Normalmap", 2D) = "bump" {}
		_HeightMap ("_HeightMap (r)", 2D) = "white" {}
		_Height ("_Height", Range (0, 1)) = 0.3
		_OffsetXHeightMap ("_OffsetXHeightMap", Range (0, 1)) = 0
		_OffsetYHeightMap ("_OffsetYHeightMap", Range (0, 1)) = 0
        _FPOW("FPOW Fresnel", Float) = 5.0
        _R0("R0 Fresnel", Float) = 0.05
		_Cutoff ("Emission strength", Range (0, 1)) = 0.5
		_BumpAmt ("Distortion", range (0, 20)) = 10
}

SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transperent" }
		GrabPass {}		
        LOD 200
		ZWrite On

CGPROGRAM

#pragma surface surf BlinnPhong alpha vertex:vert
#pragma target 3.0
#pragma glsl

sampler2D _BumpMap;
sampler2D _HeightMap;

float _BumpAmt;
sampler2D _GrabTexture;
float4 _GrabTexture_TexelSize;

float4 _Color;
float4 _RimColor;
float4 _ReflectColor;
float _Shininess;
float _FPOW;
float _R0;
float _Cutoff;
float _Height;
float _OffsetXHeightMap;
float _OffsetYHeightMap;

struct Input {
        float2 uv_BumpMap;
		float2 uv_HeightMap;
        float3 viewDir;
		float2 uv_BumpMapGlass;
		float4 proj : TEXCOORD0;
};

void vert (inout appdata_full v, out Input o) {
	UNITY_INITIALIZE_OUTPUT(Input,o);
	float4 oPos = UnityObjectToClipPos(v.vertex);
	#if UNITY_UV_STARTS_AT_TOP
		float scale = -1.0;
	#else
		float scale = 1.0;
	#endif
	o.proj.xy = (float2(oPos.x, oPos.y*scale) + oPos.w) * 0.5;
	o.proj.zw = oPos.zw;

	float4 coord = float4(v.texcoord.xy, 0 ,0);
	coord.x += _OffsetXHeightMap;
	coord.y += _OffsetYHeightMap;
	float4 tex = tex2Dlod (_HeightMap, coord);
	v.vertex.xyz += v.normal * _Height * tex.r;
}
 
void surf (Input IN, inout SurfaceOutput o) {
        o.Specular = _Shininess;
		o.Gloss = 1;
        o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		
        half fresnel = saturate(1.0 - dot(o.Normal, normalize(IN.viewDir)));
        fresnel = pow(fresnel, _FPOW);
        fresnel = _R0 + (1.0 - _R0) * fresnel;

		half fresnelRim = saturate(0.7 - dot(half3(0,0,1), normalize(IN.viewDir)));
        fresnelRim = pow(fresnelRim, _FPOW);
        fresnelRim = _R0 + (1.0 - _R0) * fresnelRim;

	    half2 offset = o.Normal.rg * _BumpAmt * _GrabTexture_TexelSize.xy;
		IN.proj.xy = offset * IN.proj.z + IN.proj.xy;
		half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(IN.proj));

		o.Emission = col.xyz * _Color + (fresnel *_ReflectColor) * _Cutoff + col.xyz * (fresnelRim * _RimColor)* _Cutoff;
        o.Alpha = _Color.a;
}
ENDCG
}

FallBack "Reflective/Bumped Diffuse"
}