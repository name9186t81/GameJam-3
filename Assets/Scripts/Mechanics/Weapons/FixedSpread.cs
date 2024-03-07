using UnityEngine;

namespace Weapons
{
	public sealed class FixedSpread : ISpreadProvider
	{
		public Vector2 GetDirection(in Vector2 originalDirection)
		{
			return originalDirection;
		}
	}
}