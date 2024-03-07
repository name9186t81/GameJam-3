using System;

using Health;

namespace Core
{
	public interface IDamageReactable
	{
		bool CanTakeDamage(DamageArgs args);
		void TakeDamage(DamageArgs args);
		event Action<DamageArgs> OnDamage;
	}
}
