Shader "Warehouse/BloodParticles"
{
 SubShader
 {
  Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
  Pass
  {
   Blend SrcAlpha OneMinusSrcAlpha
   ZWrite Off
   Cull Off
   HLSLPROGRAM
   #pragma vertex Vert
   #pragma fragment Frag
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
   struct A { float4 positionOS : POSITION; half4 color : COLOR; float2 uv : TEXCOORD0; };
   struct V { float4 positionCS : SV_POSITION; half4 color : COLOR; float2 uv : TEXCOORD0; };
   V Vert(A a) { V o; o.positionCS=TransformObjectToHClip(a.positionOS.xyz); o.color=a.color; o.uv=a.uv; return o; }
   half4 Frag(V i) : SV_Target { half4 c=i.color; c.a *= saturate(1-length(i.uv*2-1)); return c; }
   ENDHLSL
  }
 }
}
