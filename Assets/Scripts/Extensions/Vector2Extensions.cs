using UnityEngine;

public static class Vector2Extensions
{
	/// <summary>
	/// Angle in radians.
	/// </summary>
	/// <param name="vector"></param>
	/// <param name="angle"></param>
	/// <returns></returns>
	public static Vector2 Rotate(this in Vector2 vector, float angle)
	{
		float sin = Mathf.Sin(angle);
		float cos = Mathf.Cos(angle);

		return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
	}

	public static Vector2 ClampValues(this in Vector2 vector, float min, float max)
	{
		return new Vector2(Mathf.Clamp(vector.x, min, max), Mathf.Clamp(vector.y, min, max));
	}

	/// <summary>
	/// Angle in radians.
	/// </summary>
	/// <param name="angle"></param>
	/// <returns></returns>
	public static Vector2 VectorFromAngle(this float angle)
	{
		return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
	}

	/// <summary>
	/// Return angle in degrees. Use AngleRadFromVector for radians
	/// </summary>
	/// <param name="vector"></param>
	/// <returns></returns>
	public static float AngleFromVector(this in Vector2 vector)
	{
		return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
	}

	public static Vector2 Perpendicular(this in Vector2 vector)
	{
		return new Vector2(-vector.y, vector.x);
	}
	public static Vector2 Perpendicular2(this in Vector2 vector)
	{
		return new Vector2(vector.y, -vector.x);
	}

	public static float AngleRadFromVector(this in Vector2 vector)
	{
		return Mathf.Atan2(vector.y, vector.x);
	}

	public static Vector2 GetDirection(this in Vector2 p1, in Vector2 otherPoint)
	{
		return otherPoint - p1;
	}

	public static bool DistanceLess(this in Vector2 v1, in Vector2 v2, float dist)
	{
		return (v1 - v2).sqrMagnitude < dist * dist;
	}

	public static Vector2 RandomDirection()
	{
		return (Random.Range(0, Mathf.PI * 2)).VectorFromAngle();
	}

	public static Vector2 GetDirectionNormalized(this in Vector2 p1, in Vector2 otherPoint)
	{
		return p1.GetDirection(otherPoint).normalized;
	}

	public static Vector2 GetDirection(this in Vector3 p1, in Vector3 otherPoint)
	{
		return otherPoint - p1;
	}

	public static Vector2 GetDirectionNormalized(this in Vector3 p1, in Vector3 otherPoint)
	{
		return p1.GetDirection(otherPoint).normalized;
	}

	public static Vector2 SetVectorLength(this in Vector2 vector, float length)
	{
		return vector.normalized * length;
	}
}