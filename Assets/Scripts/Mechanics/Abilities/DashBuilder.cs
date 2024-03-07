using UnityEngine;

using Core;

namespace Abilities
{
	[CreateAssetMenu(fileName = "Dash ability", menuName = "GameJam/Dash")]
	public sealed class DashBuilder : AbilityBuilder
	{
		[SerializeField] private float _dashLength;
		[SerializeField] private float _dashDuration;
		[SerializeField] private float _dashCooldown;
		[SerializeField] private float _lengthMax;
		[SerializeField] private float _durationMax;
		[SerializeField] private float _cooldownMax;
		[SerializeField, Range(0, 1f)] private float _escapeTolerancy = 1f;
		[SerializeField, Range(0, 1f)] private float _attackTolerancy = 1f;
		[SerializeField, Range(0, 1f)] private float _moveTolerancy = 1f;
		[SerializeField] private AnimationCurve _speedOverTime;

		private void OnValidate()
		{
			_dashLength = Mathf.Max(0f, _dashLength);
			_dashDuration = Mathf.Max(0f, _dashDuration);
		}

		public override IAbility Build(IActor owner)
		{
			float factor = 0.5f;
			//if(owner.Controller != null && owner.Controller.Type == ControllerType.AI)
			//{
			//	var controller = owner.Controller as AIController;
			//	factor = controller.Aggresivness;
			//}

			float duration = Mathf.Lerp(_dashDuration, _durationMax, 1 - factor);
			float length = Mathf.Lerp(_dashLength, _lengthMax, 1 - factor);
			float coolDown = Mathf.Lerp(_dashCooldown, _cooldownMax, 1 - factor);

			//DashAbility ability = new DashAbility(_speedOverTime, length, duration, coolDown, 
			//	new DashSubState(new IAIAbilityInstruction[]
			//	{
			//		new EscapeDash()
			//	}));
			//todo: не забыть раскомить как переделаю
			DashAbility ability = new DashAbility(_speedOverTime, length, duration, coolDown, null);
			ability.Init(owner);
			return ability;
		}
	}
}