Shader "Custom/WaterBlur" {
    Properties {
 _blurSizeXY("BlurSizeXY", Range(0,10)) = 2
}
    SubShader {
        // Draw ourselves after all opaque geometry
        Tags { "Queue" = "Transparent" }
        // Grab the screen behind the object into _GrabTexture
        GrabPass { }
        // Render the object with the texture generated above
        Pass {

CGPROGRAM
#pragma debug
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
            sampler2D _GrabTexture : register(s0);
            float _blurSizeXY;
struct data {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
};
 
struct v2f {
    float4 position : POSITION;
    float4 screenPos : TEXCOORD0;
};
 
v2f vert(data i){
    v2f o;
    o.position = mul(UNITY_MATRIX_MVP, i.vertex);
    o.screenPos = o.position;
    return o;
}
 
half4 frag( v2f i ) : COLOR
{
    float2 screenPos = i.screenPos.xy / i.screenPos.w;
 float depth= _blurSizeXY*0.0005;
    screenPos.x = (screenPos.x + 1) * 0.5;
    screenPos.y = 1-(screenPos.y + 1) * 0.5;
    half4 sum = half4(0.0h,0.0h,0.0h,0.0h);  
    sum += tex2D( _GrabTexture, float2(screenPos.x-5.0 * depth, screenPos.y+5.0 * depth)) * 0.025;   
    sum += tex2D( _GrabTexture, float2(screenPos.x+5.0 * depth, screenPos.y-5.0 * depth)) * 0.025;
   
    sum += tex2D( _GrabTexture, float2(screenPos.x-4.0 * depth, screenPos.y+4.0 * depth)) * 0.05;
    sum += tex2D( _GrabTexture, float2(screenPos.x+4.0 * depth, screenPos.y-4.0 * depth)) * 0.05;
   
    sum += tex2D( _GrabTexture, float2(screenPos.x-3.0 * depth, screenPos.y+3.0 * depth)) * 0.09;
    sum += tex2D( _GrabTexture, float2(screenPos.x+3.0 * depth, screenPos.y-3.0 * depth)) * 0.09;
   
    sum += tex2D( _GrabTexture, float2(screenPos.x-2.0 * depth, screenPos.y+2.0 * depth)) * 0.12;
    sum += tex2D( _GrabTexture, float2(screenPos.x+2.0 * depth, screenPos.y-2.0 * depth)) * 0.12;
   
    sum += tex2D( _GrabTexture, float2(screenPos.x-1.0 * depth, screenPos.y+1.0 * depth)) *  0.15;
    sum += tex2D( _GrabTexture, float2(screenPos.x+1.0 * depth, screenPos.y-1.0 * depth)) *  0.15;
   
 
    sum += tex2D( _GrabTexture, screenPos-5.0 * depth) * 0.025;   
    sum += tex2D( _GrabTexture, screenPos-4.0 * depth) * 0.05;
    sum += tex2D( _GrabTexture, screenPos-3.0 * depth) * 0.09;
    sum += tex2D( _GrabTexture, screenPos-2.0 * depth) * 0.12;
    sum += tex2D( _GrabTexture, screenPos-1.0 * depth) * 0.15;   
    sum += tex2D( _GrabTexture, screenPos) * 0.16;
    sum += tex2D( _GrabTexture, screenPos+5.0 * depth) * 0.15;
    sum += tex2D( _GrabTexture, screenPos+4.0 * depth) * 0.12;
    sum += tex2D( _GrabTexture, screenPos+3.0 * depth) * 0.09;
    sum += tex2D( _GrabTexture, screenPos+2.0 * depth) * 0.05;
    sum += tex2D( _GrabTexture, screenPos+1.0 * depth) * 0.025;
      
 return sum/2;
}
ENDCG
        }
    }
Fallback Off
}