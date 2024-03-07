using UnityEngine;
using Core;
using System;

namespace AI
{
	[DisallowMultipleComponent(), RequireComponent(typeof(AIVision))]
	public sealed class AIController : MonoBehaviour, IController
	{
		private IActor _controlled;
		private AIVision _vision;
		private Vector2 _moveDirection;
		private Vector2 _rotation;

		public ControllerType Type => ControllerType.AI;

		public Vector2 DesiredMoveDirection => _moveDirection;

		public Vector2 DesiredRotation => _rotation;

		public event Action<ControllerAction> OnAction;

		private void Start()
		{
			if(!TryGetComponent<IActor>(out var actor))
			{
				Debug.LogError("Cannot find actor");
				return;
			}

			if(!actor.TryChangeController(this))
			{
				Debug.LogError("Cannot change controller on actor");
				return;
			}

			_controlled = actor;
		}

		public void MoveToPoint(Vector2 point)
		{
			_moveDirection = Position.GetDirectionNormalized(point);
		}

		public void LookAt(Vector2 point)
		{
			_rotation = Position.GetDirectionNormalized(point);
		}

		public AIVision Vision => _vision;
		public Vector2 Position => _controlled.Position;
		public IActor Actor => _controlled;
	}
}