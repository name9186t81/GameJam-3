using Core;

using System;

using UnityEngine;

namespace Core
{
	[DisallowMultipleComponent()]
	public sealed class Unit : MonoBehaviour, IActor
	{
		[SerializeField] private float _speed;
		private Transform _transform;
		private IController _controller;
		public Vector2 Position => _transform.position;

		public IController Controller => _controller;

		public event Action<ControllerAction> OnAction;

		private void Awake()
		{
			_transform = transform;
		}
		private void Update()
		{
			_transform.position = (Vector2)_transform.position + (this as IActor).DesiredMoveDirection * Time.deltaTime * _speed;
			//изменять позицию нужно в моторе там не нужен каст
			var rot = (this as IActor).DesiredRotation;
			_transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(rot.y, rot.x));
		}
		public bool TryChangeController(in IController controller)
		{
			if(_controller != null)
			{
				_controller.OnAction -= Act;
			}
			_controller = controller;
			_controller.OnAction += Act;
			return true;
		}

		private void Act(ControllerAction obj)
		{
			OnAction?.Invoke(obj);
		}
	}
}