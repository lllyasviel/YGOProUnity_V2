// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Copyright (C) 2014 Stephan Schaem - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

void VertShader(inout appdata_full v, out Input data)
{
	v.vertex.x += _VertexOffsetX;
	v.vertex.y += _VertexOffsetY;

	UNITY_INITIALIZE_OUTPUT(Input,data);

	float bold = step(v.texcoord1.y, 0);
	
	// Generate normal for backface
	float3 view = ObjSpaceViewDir(v.vertex);
	v.normal *= sign(dot(v.normal, view));

#if USE_DERIVATIVE
	data.param.y = 1;//v.texcoord1.y;// * _GradientScale * 1.5;
#else
	float4 vert = v.vertex;
	float4 vPosition = UnityObjectToClipPos(vert);
	float2 pixelSize = vPosition.w; // * unity_Scale.w;
	//pixelSize /= float2(_ScaleX * _ScreenParams.x * UNITY_MATRIX_P[0][0], _ScaleY * _ScreenParams.y * UNITY_MATRIX_P[1][1]);
	pixelSize /= float2(_ScaleX, _ScaleY) * mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy);
	float scale = rsqrt(dot(pixelSize, pixelSize));
	scale *= abs(v.texcoord1.y) * _GradientScale * 1.5;
	//scale = lerp(scale/8.0, scale, abs(dot(v.normal.xyz, normalize(ObjSpaceViewDir(vert)))));
	scale = lerp(scale * (1 - _PerspectiveFilter), scale, abs(dot(UnityObjectToWorldNormal(v.normal.xyz), normalize(ObjSpaceViewDir(vert)))));
	data.param.y = scale;
#endif

	//float opacity = v.color.a;

	data.param.x = (lerp(_WeightNormal, _WeightBold, bold)) / _GradientScale + _FaceDilate*_ScaleRatioA*.5;

	v.texcoord1.xy = UnpackUV(v.texcoord1.x);
	data.viewDirEnv =   mul((float3x3)_EnvMatrix, WorldSpaceViewDir(v.vertex));
}

void PixShader(Input input, inout SurfaceOutput o)
{

#if USE_DERIVATIVE | BEVEL_ON
	float3 delta = float3(1.0/_TextureWidth, 1.0/_TextureHeight, 0.0);

	float4 smp4x = {tex2D(_MainTex, input.uv_MainTex - delta.xz).a,
					tex2D(_MainTex, input.uv_MainTex + delta.xz).a,
					tex2D(_MainTex, input.uv_MainTex - delta.zy).a,
					tex2D(_MainTex, input.uv_MainTex + delta.zy).a};
#endif

#if USE_DERIVATIVE
	// Screen space scaling reciprocal with anisotropic correction
	float2 edgeNormal = Normalize(float2(smp4x.x - smp4x.y, smp4x.z - smp4x.w));
	float2 res = float2(_TextureWidth * input.param.y, _TextureHeight);
	float2 tdx = ddx(input.uv_MainTex)*res;
	float2 tdy = ddy(input.uv_MainTex)*res;
	float lx = length(tdx);
	float ly = length(tdy);
	float s = sqrt(min(lx, ly) / max(lx,ly));
	s = lerp(1, s, abs(dot(normalize(tdx+tdy), edgeNormal)));
	float scale = rsqrt(abs(tdx.x * tdy.y - tdx.y * tdy.x)) * (_GradientScale * 2) * s;
#else
	float scale = input.param.y;
#endif

	// Signed distance
	float c = tex2D(_MainTex, input.uv_MainTex).a;
	float sd = (.5 - c - input.param.x) * scale + .5;
	float outline = _OutlineWidth*_ScaleRatioA * scale;
	float softness = _OutlineSoftness*_ScaleRatioA * scale;

	// Color & Alpha
	float4 faceColor = _FaceColor;
	float4 outlineColor = _OutlineColor;
	faceColor *= input.color;
	outlineColor.a *= input.color.a;
	faceColor *= tex2D(_FaceTex, float2(input.uv2_FaceTex.x + _FaceUVSpeedX * _Time.y, input.uv2_FaceTex.y + _FaceUVSpeedY * _Time.y));
	outlineColor *= tex2D(_OutlineTex, float2(input.uv2_FaceTex.x + _OutlineUVSpeedX * _Time.y, input.uv2_FaceTex.y + _OutlineUVSpeedY * _Time.y));
	faceColor = GetColor(sd, faceColor, outlineColor, outline, softness);
	faceColor.rgb /= faceColor.a;

#if BEVEL_ON
	// Face Normal
	float3 n = GetSurfaceNormal(smp4x, input.param.x);

	// Bumpmap
	float3 bump = UnpackNormal(tex2D(_BumpMap, input.uv2_FaceTex)).xyz;
	bump *= lerp(_BumpFace, _BumpOutline, saturate(sd + outline * 0.5));
	bump = lerp(float3(0,0,1), bump, faceColor.a);
	n = normalize(n - bump);

	// Cubemap reflection
	fixed4 reflcol = texCUBE(_Cube, reflect(input.viewDirEnv, mul((float3x3)unity_ObjectToWorld,n)));		
	float3 emission = reflcol.rgb * lerp(_ReflectFaceColor.rgb, _ReflectOutlineColor.rgb, saturate(sd + outline * 0.5)) * faceColor.a;
#else
	float3 n = float3(0,0,-1);
	float3 emission = float3(0,0,0);
#endif

#if GLOW_ON
	float4 glowColor = GetGlowColor(sd, scale);
	glowColor.a *= input.color.a;
	emission += glowColor.rgb*glowColor.a;
	faceColor = BlendARGB(glowColor, faceColor);
	faceColor.rgb /= faceColor.a;
#endif

	// Set Standard output structure
	o.Albedo = faceColor.rgb;
	o.Normal = -n;
	o.Emission = emission;
	o.Specular = lerp(_FaceShininess, _OutlineShininess, saturate(sd + outline * 0.5));
	o.Gloss = 1;
	o.Alpha = faceColor.a;
}
