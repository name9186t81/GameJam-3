using Core;

using System;
using Weapons;
using UnityEngine;

namespace Health
{
	public sealed class DamageArgs : EventArgs
	{
		public readonly IActor Sender;
		public readonly int Damage;
		public readonly DamageFlags DamageFlags;
		public readonly IWeapon Weapon;
		public Vector2 HitPosition;
		public Vector2 SourcePosition;
		public float Radius = 0;

		public DamageArgs(IActor sender, int damage, DamageFlags damageFlags, IWeapon weapon = null, float radius = 0)
		{
			Sender = sender;
			Damage = damage;
			DamageFlags = damageFlags;
			Weapon = weapon;
			Radius = radius;
		}
	}

	[Flags]
	public enum DamageFlags
	{
		Unknown = 0,
		Kinetic = 1,
		Explosive = 2,
		Fire = 4,
		Melee = 8,
		Ranged = 16,
		NoWeapon = 32,
		Heal = 64
	}
}
