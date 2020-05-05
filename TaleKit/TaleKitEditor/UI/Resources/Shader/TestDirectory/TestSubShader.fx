sampler2D input : register(s0);
float threshold : register(c1);
 
float4 main(float2 uv : TEXCOORD) : COLOR
{
   float4 color = tex2D( input, uv );
   
   color.r = 1.0;
 
  return color;
}