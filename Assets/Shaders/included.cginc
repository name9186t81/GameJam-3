void IsIntersecting(Texture2D tex, SamplerState SS, float2 uv, float angle, float length, out float OUT)
{
	OUT = SAMPLE_TEXTURE2D(tex, SS, uv).r;
}