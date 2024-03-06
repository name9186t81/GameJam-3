using System;

using Core;

namespace Abilities
{
	public interface IAbility
	{
		AbilityType Type { get; }
		bool CanUse();
		void Use();
		void Update(float dt);
		void Init(IActor actor);
		/// <summary>
		/// Instruction for ai of how to use an ability. Can be null.
		/// </summary>
		IAIAbilityInstruction AIAbilityInstruction { get; }

		/// <summary>
		/// Shows when ability is ready for use. 1 - ready, 0 - not.
		/// </summary>
		float Readiness { get; }
		event Action OnActivate;
		event Action OnDeactivate;
	}

	public enum AbilityType
	{
		Passive,
		Instant,
		Directional
	}
}