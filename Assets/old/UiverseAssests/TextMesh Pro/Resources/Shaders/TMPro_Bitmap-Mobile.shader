// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TMPro/Mobile/Bitmap" {

Properties {
	_MainTex		("Font Atlas", 2D) = "white" {}
	_Color			("Text Color", Color) = (1,1,1,1)
	_DiffusePower	("Diffuse Power", Range(1.0,4.0)) = 1.0
}

SubShader {

	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

	Lighting Off
	Cull Off
	ZTest Always
	ZWrite Off
	Fog { Mode Off }
	Blend SrcAlpha OneMinusSrcAlpha

	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest

		#include "UnityCG.cginc"

		struct appdata_t {
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord0 : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
		};

		struct v2f {
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord0 : TEXCOORD0;
		};

		sampler2D 	_MainTex;
		fixed4		_Color;
		float		_DiffusePower;

		v2f vert (appdata_t v)
		{
			v2f o;
			o.vertex = UnityPixelSnap(UnityObjectToClipPos(v.vertex));
			//o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.color = v.color;
			if(o.color.a > .5) o.color.a -= .5;
			o.color.a *= 2.0;
			o.color *= _Color;
			o.color.rgb *= _DiffusePower;
			o.texcoord0 = v.texcoord0;
			return o;
		}

		fixed4 frag (v2f i) : COLOR
		{
			return fixed4(i.color.rgb, i.color.a * tex2D(_MainTex, i.texcoord0).a);
		}
		ENDCG
	}
}

SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Lighting Off Cull Off ZTest Always ZWrite Off Fog { Mode Off }
	Blend SrcAlpha OneMinusSrcAlpha
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord0
	}
	Pass {
		SetTexture [_MainTex] {
			constantColor [_Color] combine constant * primary, constant * texture
		}
	}
}
}
