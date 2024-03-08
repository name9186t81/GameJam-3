using AI.States;

using UnityEngine;
namespace AI
{
	[CreateAssetMenu(fileName = "PetSlime", menuName = "GameJam/Slime AI")]
	public sealed class PetSlimeAIMachine : UtilityMachineBuilder
	{
		[SerializeField] private float _timeToReturn;
		[SerializeField] private float _variety;

		public override UtilityMachine Build(AIController controller)
		{
			return new UtilityMachine(new IUtility[]
			{
				new IdleWalk(0, controller.Actor.Scale * 3f, controller.Actor.Scale * 2f),
				new TouchAttack(),
				new ReturnToOwner(Random.Range(_timeToReturn - _timeToReturn * _variety, _timeToReturn + _timeToReturn * _variety))
			});
		}

		private void OnValidate()
		{
			_timeToReturn = Mathf.Max(0f, _timeToReturn);
			_variety = Mathf.Max(0f, _variety);
		}
	}
}
