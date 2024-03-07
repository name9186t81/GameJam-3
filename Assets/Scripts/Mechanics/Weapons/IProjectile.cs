using System;
using UnityEngine;
using Core;
using Health;

namespace Weapons
{
	public interface IProjectile
	{
		float Speed { get; }
		IActor Source { get; }
		Vector2 Direction { get; }
		Vector2 Position { get; }
		Vector2 StartPosition { get; }
		ProjectileFlags ProjectileFlags { get; }
		event Action OnInit;
		bool TryChangeDirection(in Vector2 newDirection);
		void Init(Pool<IProjectile> pool, DamageArgs args, int teamNumber, in Vector2 startPos, in Vector2 direction, IActor owner, float speedModifier);
		void Destroy();
	}

	[Flags]
	public enum ProjectileFlags
	{
		None = 0,
		NotMutable = 1,
		NoSource = 2,
		HaveTeam = 4,
		Frozen = 8,
		GoThroughTargets = 16
	}
}
