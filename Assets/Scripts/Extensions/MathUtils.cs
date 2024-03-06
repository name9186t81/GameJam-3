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

    public static float map01(this float X, float C, float D)
    {
        return X * (D - C) + C;
    }

    public static float map(this float X, float A, float B, float C, float D)
    {
        return (X - A) / (B - A) * (D - C) + C;
    }
}

public class FloatSmoothDamp
{
    private float _velocity;
    private float _value;

    public float SmoothTime;
    public float Value { get { return _value; } set { _value = value; _velocity = 0; } }

    public float Update(float target, float dt = -1)
    {
        if (dt == -1)
            dt = Time.deltaTime;

        _value = Mathf.SmoothDamp(_value, target, ref _velocity, SmoothTime, float.MaxValue, dt);
        return _value;
    }

    public FloatSmoothDamp(float time, float startValue = 0)
    {
        SmoothTime = time;
        _value = startValue;
    }
}

public class Vector2SmoothDamp
{
    private FloatSmoothDamp _x;
    private FloatSmoothDamp _y;

    public float SmoothTime { get { return _x.SmoothTime; } set { _x.SmoothTime = value; _y.SmoothTime = value; } }
    public Vector2 Value { get { return new Vector2(_x.Value, _y.Value); } set { _x.Value = value.x; _y.Value = value.y; } }

    public Vector2SmoothDamp(float time, Vector2 startValue)
    {
        _x = new FloatSmoothDamp(time, startValue.x);
        _y = new FloatSmoothDamp(time, startValue.y);
    }

    public Vector2 Update(Vector2 target)
    {
        return new Vector2(_x.Update(target.x), _y.Update(target.y));
    }
}