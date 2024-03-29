using System;

using UnityEngine;

namespace Core
{
	public interface IActor
	{
		public Vector2 DesiredMoveDirection => Controller == null ? Vector2.zero : Controller.DesiredMoveDirection;
		public Vector2 DesiredRotation => Controller == null ? Vector2.zero : Controller.DesiredRotation;
		event Action<ControllerAction> OnAction;
		float Scale { get; } //radius of the unit
		float Radius { get; }
		Vector2 Position { get; }
		IController Controller { get; }
		string Name { get; }
		bool TryChangeController(in IController controller);
		event Action OnInit;
	}
}