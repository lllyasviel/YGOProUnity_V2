Shader "Custom/rimlight" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
		_RimColor ("Rim Color", Color) = (1,1,1,0.0)
      	_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
      	_RimIntensity ("Rim Intensity", Range(0.5,8.0)) = 3.0
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float4 _Color;
		float4 _RimColor;
      	float _RimPower;
      	float _RimIntensity;
		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};
	
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
          	o.Emission = _RimColor.rgb * pow (rim, _RimPower) * _RimIntensity;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
