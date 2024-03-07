using Core;

using System;

namespace Health
{
	public interface IHealth : IDamageReactable
	{
		IActor Actor { set; get; }
		int CurrentHealth { get; }
		int MaxHealth { get; }
		event Action<DamageArgs> OnDeath;
		HealthFlags Flags { get; set; }
	}

	[Flags]
	public enum HealthFlags
	{
		None = 0,
		Invincible = 1,
		Reinforced = 2,
		FriendlyFireDisabled = 4
	}
}
