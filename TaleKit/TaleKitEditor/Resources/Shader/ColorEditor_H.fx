#include "Library\Global.fxlib"

float4 main(float2 uv : TEXCOORD) : COLOR
{
   float4 color = float4(HSVToColor(half3(uv.y, 1, 1)), 1.0);
 
  return color;
}