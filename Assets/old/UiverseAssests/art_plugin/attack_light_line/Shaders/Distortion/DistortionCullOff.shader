Shader "Effects/Distortion/CullOff+1" {
	Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "black" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
		_ColorStrength ("Color Strength", Float) = 1
		_BumpAmt ("Distortion", range (0, 100)) = 10
		
}

SubShader {
        Tags { "Queue"="Transparent+1" "RenderType"="Transperent" }
		GrabPass {}	
        LOD 200
		ZWrite On
		Cull Off
Fog { Mode Off}


CGPROGRAM

#pragma surface surf Lambert alpha vertex:vert

sampler2D _MainTex;
sampler2D _BumpMap;

float _BumpAmt;
float _ColorStrength;
sampler2D _GrabTexture;
float4 _GrabTexture_TexelSize;

float4 _Color;

struct Input {
		float2 uv_MainTex;
        float2 uv_BumpMap;
		float4 proj : TEXCOORD0;
};

void vert (inout appdata_full v, out Input o) {
	UNITY_INITIALIZE_OUTPUT(Input,o);
	float4 oPos = mul(UNITY_MATRIX_MVP, v.vertex);
	#if UNITY_UV_STARTS_AT_TOP
		float scale = -1.0;
	#else
		float scale = 1.0;
	#endif
	o.proj.xy = (float2(oPos.x, oPos.y*scale) + oPos.w) * 0.5;
	o.proj.zw = oPos.zw;
	
}
 
void surf (Input IN, inout SurfaceOutput o) {
        o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	   
	    half2 offset = o.Normal.rg * _BumpAmt * _GrabTexture_TexelSize.xy;
		IN.proj.xy = offset * IN.proj.z + IN.proj.xy;
		half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(IN.proj));

		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Emission = col.xyz + tex*_ColorStrength;
        o.Alpha = _Color.a;
}
ENDCG
}

FallBack "Effects/Distortion/Free/CullOff"
}