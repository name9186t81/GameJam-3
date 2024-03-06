using System;

using UnityEngine;

namespace Core
{
	public interface IActor
	{
		public Vector2 DesiredMoveDirection => Controller == null ? Vector2.zero : Controller.DesiredMoveDirection;
		public Vector2 DesiredRotation => Controller == null ? Vector2.zero : Controller.DesiredRotation;
		event Action<ControllerAction> OnAction;
		Vector2 position { get; }
		IController Controller { get; }
		bool TryChangeController(in IController controller);
	}
}