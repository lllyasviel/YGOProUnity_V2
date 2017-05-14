Shader "Effects/TransperentDiffuseRimSpecular" {
Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
        _Shininess ("Shininess", Range (0.01, 3)) = 0.078125
        _RimColor ("Rim Color", Color) = (1,1,1,0.5)
		_MainTex ("Main Tex", 2D) = "bump" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _FPOW("FPOW Fresnel", Float) = 5.0
        _R0("R0 Fresnel", Float) = 0.05
		_Emission ("Emission strength", Range (0, 1)) = 0.5
		_Opacity ("Opacity", range (-1, 1)) = 0
		
}

SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
		ZWrite On

CGPROGRAM

#pragma surface surf BlinnPhong alpha

sampler2D _MainTex;
sampler2D _BumpMap;
float _Opacity;
float4 _Color;
float4 _RimColor;
float _Shininess;
float _FPOW;
float _R0;
float _Emission;

struct Input {
		float2 uv_MainTex;
        float2 uv_BumpMap;
        float3 viewDir;
};

void surf (Input IN, inout SurfaceOutput o) {
        o.Specular = _Shininess;
		o.Gloss = 1;
        o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

        half fresnel = saturate(1.0 - dot(o.Normal, normalize(IN.viewDir)));
        fresnel = pow(fresnel, _FPOW);
        fresnel = _R0 + (1.0 - _R0) * fresnel;

		o.Albedo = c/2;
		o.Emission = c + (fresnel *_RimColor) * _Emission;
        o.Alpha = c.a + _Opacity;
}
ENDCG
}

FallBack "Self-Illumin/Diffuse"
}