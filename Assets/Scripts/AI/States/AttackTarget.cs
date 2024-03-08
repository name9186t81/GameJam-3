using Core;

using UnityEngine;

namespace AI.States
{
	public sealed class AttackTarget : IUtility
	{
		private AIController _controller;
		public void Execute()
		{
			if (_controller.IsTargetNull) return;
			var target = _controller.CurrentTarget;

			if (!_controller.Vision.CanSeeTarget(target))
			{
				return;
			}
			Vector2 pos = target.Position;

			_controller.LookAt(pos);

			if (_controller.Weapon.Rotatable)
			{
				var dir = _controller.Position.GetDirectionNormalized(pos);
				_controller.Weapon.LookRotation = Vector2.MoveTowards(_controller.Weapon.LookRotation, dir, 1 * Time.deltaTime);
				//^ говно переделать
			}

			if (_controller.IsEffectiveToFire(target))
			{
				_controller.InitCommand(ControllerAction.Fire);
				if (_controller.Position.DistanceLess(pos, _controller.Weapon.UseRange / 2))
				{
					_controller.MoveToPoint(-target.Position);
				}
				_controller.StopMoving();
			}
			else
			{
				_controller.MoveToPoint(target.Position);
			}

		}

		public float GetEffectivness()
		{
			return 0f;
		}

		public void Init(AIController controller)
		{
			_controller = controller;
		}

		public void PreExecute()
		{
			_controller.StopMoving();
		}

		public void Undo()
		{
		}
	}
}
