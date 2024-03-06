using UnityEngine;

public static class MathUtils
{
	public static float Delta(this float val, float max)
	{
		return val / max;
	}
	public static float Area(this AnimationCurve curve, float end = 1f, bool ignoreNegative = false)
	{
		if (curve == null) return 0f;

		const float step = 0.001f;

		float preValue = curve.Evaluate(step);
		float area = 0f;
		for (float st = step; st < end; st += step)
		{
			float current = curve.Evaluate(st);

			if (ignoreNegative && current < 0) continue;

			area += (current + preValue) * step * 0.5f;
			preValue = current;
		}

		//area += (curve.Evaluate(end) + preValue) * step * 0.5f;
		return area;
	}
}

