// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ArrowShader" 
{
	Properties
	{
		_MainTex ("MianTex (RGB) Trans (A)", 2D) = "white" {}
		_BlendColor ("BlendColor", Color) = (1,1,1,1)
		
		_Alpha ("Alpha", Range(0,1)) = 1
		_ToGray ("ToGray", Range(0,1)) = 0
	}

	SubShader
	{
		Tags { "Queue"="Transparent"
		       "IgnoreProjector"="True"
		       "RenderType"="Transparent" }
//		Tags { "Queue"="Geometry"
//	           "IgnoreProjector"="True"
//	           "RenderType"="Opaque" }
		LOD 100
		//Lighting Off
		//Cull Off
		Cull Back
		ZTest Always
		//ZTest Off
		//ZTest On
		//ZWrite Off
		//Fog { Mode Off }
		Lighting Off
		//ZWrite Off
		SeparateSpecular Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass 
		{
			CGPROGRAM
			#pragma vertex vert_arrow
			#pragma fragment frag_arrow
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			struct appdata_t 
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _BlendColor;
			uniform float _Alpha;
			uniform float _ToGray;
			
			v2f vert_arrow (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			float4 frag_arrow (v2f i) : COLOR
			{
			    float4 texCol = tex2D(_MainTex, i.texcoord);
				float4 col = _BlendColor * texCol;
                float gray = (col.r + col.g + col.b)/3;
                col.rgb = lerp(col.rgb, float3(gray,gray,gray), _ToGray);
                col.a = texCol.a * _Alpha;
				return col;
			}
			ENDCG
			
		}//Pass
		
	}//SubShader
	
}//Shader

