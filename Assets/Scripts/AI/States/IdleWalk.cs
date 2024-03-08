using UnityEngine;

namespace AI.States
{
	public sealed class IdleWalk : IUtility
	{
		private AIController _controller;
		private readonly float _walkCooldown;
		private readonly float _pointPickRadius;
		private readonly float _pointPickRadiusMin;

		private Vector2 _point;
		private float _elapsedCooldown;
		private bool _isWalking;

		private const int MAX_POINT_PICK_TRIES = 10;

		public IdleWalk(float walkCooldown, float pointPickRadius, float pointPickRadiusMin)
		{
			_walkCooldown = walkCooldown;
			_pointPickRadius = pointPickRadius;
			_pointPickRadiusMin = pointPickRadiusMin;
		}

		public void Execute()
		{
			_controller.MoveToPoint(_point);
			_controller.LookAt(_point);
			if (_controller.Weapon != null && _controller.Weapon.Rotatable)
			{
				var dir = _controller.Position.GetDirectionNormalized(_point);
				_controller.Weapon.LookRotation = Vector2.MoveTowards(_controller.Weapon.LookRotation, dir, 0.5f);
			}

			if (_controller.Position.DistanceLess(_point, _controller.Actor.Scale)) //я буду молится что диаметр всех персов равен 1
																																							//я больше не буду молится потому что добавили размер персов
			{
				_isWalking = false;
				_elapsedCooldown = _walkCooldown;
			}
		}

		public float GetEffectivness()
		{
			_elapsedCooldown -= Time.deltaTime;
			_elapsedCooldown = Mathf.Clamp(_elapsedCooldown, 0, _walkCooldown);

			return !_controller.IsTargetNull ? -100f : _isWalking ? 2f : _elapsedCooldown == 0 ? 1f : -1f;
		}

		public void Init(AIController controller)
		{
			_controller = controller;
		}

		public void PreExecute()
		{
			_isWalking = true;
			for (int i = 0; i < MAX_POINT_PICK_TRIES; i++)
			{
				if (TryPickPoint()) return;
			}

			_isWalking = false;
			_elapsedCooldown = _walkCooldown;
		}

		private bool TryPickPoint()
		{
			_point = Vector2Extensions.RandomDirection() * PointPickRadius + _controller.Position;
			return _controller.Vision.CanSeePoint(_point);
		}

		public void Undo()
		{
			_elapsedCooldown = _walkCooldown;
			_isWalking = false;
		}

		public float PointPickRadius => Random.Range(_pointPickRadiusMin, _pointPickRadius) * _controller.Actor.Scale;
	}
}
