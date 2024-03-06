using System;

using UnityEngine;

using Core;
using Movement;

namespace Abilities
{
	public sealed class DashAbility : IDirectionalAbility, IForce
	{
		private Motor _motor;
		private Vector2 _direction;
		private Vector2 _factor;
		private bool _isActive;
		private bool _cooling;
		private float _elapsed;
		private float _elapsedCooldown;
		private ForceState _state;
		private float _readiness;

		private readonly IAIAbilityInstruction _instruction;
		private readonly AnimationCurve _growCurve;
		private readonly float _distance;
		private readonly float _duration;
		private readonly float _cooldown;
		private readonly float _area;

		public DashAbility(AnimationCurve growCurve, float distance, float duration, float cooldown, IAIAbilityInstruction instruction)
		{
			_growCurve = growCurve;
			_distance = distance;
			_duration = duration;
			_area = growCurve.Area();
			_instruction = instruction;
			_cooldown = cooldown;
		}

		public Vector2 Direction { set => _direction = value; }

		public AbilityType Type => AbilityType.Directional;

		public IAIAbilityInstruction AIAbilityInstruction => _instruction;

		public ForceState State { get => _state; set => _state = value; }

		public event Action OnActivate;
		public event Action OnDeactivate;

		public bool CanUse()
		{
			return !(_isActive || _cooling);
		}

		public void Init(IActor actor)
		{
			if(actor is IProvider<Motor> prov)
			{
				_motor = prov.Value;
			}
			else
			{
				Debug.LogWarning("Actor " + actor + " is not movable");
			}

			_instruction.SetAbility(this);
		}

		public void Update(float dt)
		{
			if (!_cooling) return;

			_elapsedCooldown += dt;
			_readiness = _elapsedCooldown / _cooldown;
			if (_elapsedCooldown > _cooldown)
			{
				_cooling = false;
				_elapsedCooldown = 0f;
			}
		}

		public void Use()
		{
			if (!CanUse()) return;

			_isActive = true;
			_factor = _direction * _distance / (_area * _duration);
			_elapsed = 0f;
			_motor?.AddForce(this);
			OnActivate?.Invoke();
		}

		public Vector2 GetForce(Vector2 worldPos)
		{
			_elapsed += Time.deltaTime;
			float delta = _elapsed.Delta(_duration);
			if (delta > 1f)
			{
				_isActive = false;
				_state = ForceState.Finished;
				_elapsed = 0f;
				_motor.RemoveForce(this);
				OnDeactivate?.Invoke();
				_cooling = true;
			}

			return _factor * _growCurve.Evaluate(delta);
		}

		public float Length => _distance;

		public float Readiness => _readiness;
	}
}