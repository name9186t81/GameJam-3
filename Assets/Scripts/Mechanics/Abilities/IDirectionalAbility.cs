using UnityEngine;

namespace Abilities
{
	public interface IDirectionalAbility : IAbility
	{
		Vector2 Direction { set; }
		public abstract PrefferedDirectionSource DirectionSource { get; }

		public enum PrefferedDirectionSource
        {
			MoveDirection,
			CursorDirection
        }
	}
}