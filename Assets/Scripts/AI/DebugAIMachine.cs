using AI.States;

using UnityEngine;

namespace AI
{
	[CreateAssetMenu(fileName = "DebugAI", menuName = "GameJam/Debug AI")]
	internal class DebugAIMachine : UtilityMachineBuilder
	{
		public override UtilityMachine Build(AIController controller)
		{
			return new UtilityMachine(new IUtility[]
			{
				new AttackTarget(),
				new IdleWalk(1f, 5f, 2f)
			});
		}
	}
}
