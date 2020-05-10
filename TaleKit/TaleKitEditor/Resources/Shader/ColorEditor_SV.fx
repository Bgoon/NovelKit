#include "Library\Global.fxlib"

float hue : register(c1);
 
float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 color = float4(HSVToColor(half3(hue / 360.0, uv.x, 1.0 - uv.y)), 1.0);
	
	return color;
}