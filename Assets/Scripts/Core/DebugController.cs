using System;

using UnityEngine;

namespace Core
{
	public sealed class DebugController : MonoBehaviour, IController
	{
		[SerializeField] private Unit _unit;
		private Vector2 _direction;
		private Vector2 _rotation;

		public Vector2 DesiredMoveDirection => _direction;
		public Vector2 DesiredRotation => _rotation;

		public event Action<ControllerAction> OnAction;

		private void Awake()
		{
			_unit.TryChangeController(this);
		}
		private void Update()
		{
			Vector2 dir = Vector2.zero;
			if (Input.GetKey(KeyCode.W))
			{
				dir += Vector2.up;
			}
			if(Input.GetKey(KeyCode.S))
			{
				dir += Vector2.down;
			}
			if (Input.GetKey(KeyCode.A))
			{
				dir += Vector2.left;
			}
			if(Input.GetKey(KeyCode.D))
			{
				dir += Vector2.right;
			}
			_direction = dir;

			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			_rotation = mousePos - (Vector2)_unit.transform.position;
		}
	}
}
