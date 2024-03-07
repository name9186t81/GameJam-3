using Core;

using System.Collections.Generic;

using UnityEngine;

namespace Movement
{
	public sealed class Motor
	{
		private List<IForce> _forces = new List<IForce>();
		private readonly IMovable _body;
		private readonly IActor _actor;
		private readonly float _baseMovementSpeed;
		private readonly float _baseRotationSpeed;

		public Motor(float baseMovementSpeed, float baseRotationSpeed, IMovable body, IActor actor)
		{
			_baseMovementSpeed = baseMovementSpeed;
			_baseRotationSpeed = baseRotationSpeed;
			_body = body;
			_actor = actor;
		}

		public void Update(float dt)
		{
			UpdateForces();

			Vector2 totalForce = Vector2.zero;
			for(int i = 0, length = _forces.Count; i < length; ++i)
			{
				totalForce += _forces[i].GetForce(_body.Position);
			}

			Vector2 movementForce = _actor.DesiredMoveDirection * _baseMovementSpeed;
			_body.Velocity = movementForce + totalForce;

			float angle = Mathf.Atan2(_actor.DesiredRotation.y, _actor.DesiredRotation.x) * Mathf.Rad2Deg - 90;
			_body.Rotation = Mathf.MoveTowardsAngle(_body.Rotation, angle, _baseRotationSpeed * dt);
		}

		private void UpdateForces()
		{
			List<IForce> forces = new List<IForce>();

			for(int i = 0, length = _forces.Count; i < length; ++i)
			{
				if (_forces[i].State == ForceState.Running)
				{
					forces.Add(_forces[i]);
				}
			}

			_forces = forces;
		}

		public void AddForce(IForce force)
		{
			_forces.Add(force);
			force.State = ForceState.Running;
		}

		public bool RemoveForce(IForce force)
		{
			if (_forces.Contains(force))
			{
				force.State = ForceState.Finished;
				return true;
			}
			return false;
		}
	}
}