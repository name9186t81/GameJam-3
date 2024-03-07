using System;
using System.Collections.Generic;

using UnityEngine;

namespace Movement
{
	public sealed class ParameterizedForce : IForce
	{
		private Dictionary<ParameterizedForceKey, object> _parameters = new Dictionary<ParameterizedForceKey, object>();
		private Func<Vector2, Vector2> _forceFunc;
		private ForceState _state;
		public Func<Vector2, Vector2> ForceFunc => _forceFunc;

		public ForceState State { get => _state; set => _state = value; }

		public ParameterizedForce() { }

		public ParameterizedForce SetForce(Func<Vector2, Vector2> force)
		{
			_forceFunc = force;
			return this;
		}

		public Vector2 GetForce(Vector2 worldPos)
		{
			return _forceFunc.Invoke(worldPos);
		}

		public object this[ParameterizedForceKey key]
		{
			set
			{
				if (_parameters.TryGetValue(key, out _))
				{
					_parameters[key] = value;
				}
				else
				{
					_parameters.Add(key, value);
				}
			}
			get => _parameters[key];
		}
	}

	public enum ParameterizedForceKey
	{
		PushPosition,
		PushDirection,
		ElapsedTime,
		MaxTime,
		Motor,
		Strength
	}
}
