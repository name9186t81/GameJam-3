using UnityEngine;

namespace AI
{
	public abstract class UtilityMachineBuilder : ScriptableObject
	{
		public abstract UtilityMachine Build(AIController controller);
	}
}