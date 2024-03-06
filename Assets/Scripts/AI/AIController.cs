using UnityEngine;
using Core;
using System;

namespace AI
{
	[DisallowMultipleComponent()]
	public sealed class AIController : MonoBehaviour, IController
	{
		private IActor _controlled;
		public ControllerType Type => ControllerType.AI;

		public Vector2 DesiredMoveDirection => throw new NotImplementedException();

		public Vector2 DesiredRotation => throw new NotImplementedException();

		public event Action<ControllerAction> OnAction;

		public Vector2 Position => _controlled.Position;
		public IActor Actor => _controlled;
	}
}