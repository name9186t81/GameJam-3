using UnityEngine;
using Core;

namespace Abilities
{
	public abstract class AbilityBuilder : ScriptableObject
	{
		public abstract IAbility Build(IActor owner);
	}
}