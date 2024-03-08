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
		private readonly bool _faceDirectionToMove;

		public Motor(float baseMovementSpeed, float baseRotationSpeed, IMovable body, IActor actor, bool faceDirectionToMove = false)
		{
			_baseMovementSpeed = baseMovementSpeed;
			_baseRotationSpeed = baseRotationSpeed;
			_body = body;
			_actor = actor;
			_faceDirectionToMove = faceDirectionToMove;
		}

		public void Update(float dt)
		{
			UpdateForces();

			Vector2 totalForce = Vector2.zero;
			for(int i = 0, length = _forces.Count; i < length; ++i)
			{
				totalForce += _forces[i].GetForce(_body.Position);
			}


			float angle = Mathf.Atan2(_actor.DesiredRotation.y, _actor.DesiredRotation.x) * Mathf.Rad2Deg - 90;
			_body.Rotation = Mathf.MoveTowardsAngle(_body.Rotation, angle, _baseRotationSpeed * dt);

			Vector2 movementForce = _actor.DesiredMoveDirection * _baseMovementSpeed;
			if (_faceDirectionToMove)
			{
				if(Vector2.Dot(_actor.DesiredRotation, ((_body.Rotation + 90) * Mathf.Deg2Rad).VectorFromAngle()) < 0.9f) //не спрашивать
				{
					_body.Velocity = Vector2.zero;
					return;
				}
			}
			_body.Velocity = movementForce + totalForce;
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