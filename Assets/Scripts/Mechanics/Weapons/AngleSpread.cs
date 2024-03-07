using UnityEngine;

namespace Weapons
{
	public sealed class AngleSpread : ISpreadProvider
	{
		private readonly float _iterations;
		private readonly int _offset;
		private readonly float _angle;
		private int _currentIteration;

		public AngleSpread(int iterations, float angle, int offset = 0)
		{
			_iterations = iterations;
			_angle = angle;
			_offset = offset;
		}

		public Vector2 GetDirection(in Vector2 originalDirection)
		{
			float angle = Mathf.Lerp(-_angle / 2, _angle / 2, (_currentIteration + _offset) / _iterations);
			if (++_currentIteration > (_iterations - 1)) _currentIteration = 0;
			return originalDirection.Rotate(angle);
		}
	}
}
