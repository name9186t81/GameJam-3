using System;

using UnityEngine;

namespace Core
{
	public interface IController
	{
		Vector2 DesiredMoveDirection { get; }
		Vector2 DesiredRotation { get; }
		event Action<ControllerAction> OnAction;
	}
}