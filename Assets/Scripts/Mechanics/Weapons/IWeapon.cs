using System;
using Core;

using UnityEngine;

namespace Weapons
{
	public interface IWeapon : IActorComponent
	{
		bool CanAttack { get; }
		float EffectiveRange { get; }
		float UseRange { get; }
		WeaponFlags Flags { get; }
		bool IsInited { get; }
		void Init(IActor actor);
		bool Rotatable { get; }
		Vector2 LookRotation { get; set; }
		event Action OnInit;
		event Action OnAttack;
		event Action OnAttackEnded;
	}

	[Flags]
	public enum WeaponFlags
	{
		None = 0,
		CanParry = 1,
		Melee = 2,
		Ranged = 4,
		Mixed = 8,
		PreAim = 16
	}
}
