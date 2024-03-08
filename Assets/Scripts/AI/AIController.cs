using UnityEngine;
using Core;
using System;
using Health;
using Weapons;
using Movement;

namespace AI
{
	[DisallowMultipleComponent(), RequireComponent(typeof(AIVision))]
	public sealed class AIController : MonoBehaviour, IController
	{
		[SerializeField] private UtilityMachineBuilder _utilities;
		[SerializeField] private float _fireThreshold;
		private IActor _controlled;
		private AIVision _vision;
		private Vector2 _moveDirection;
		private Vector2 _rotation;
		private IWeapon _weapon;
		private UtilityMachine _utilityMachine;

		public ControllerType Type => ControllerType.AI;

		public Vector2 DesiredMoveDirection => _moveDirection;

		public Vector2 DesiredRotation => _rotation;

		public event Action<ControllerAction> OnAction;

		public IHealth TargetHealth { get; private set; }
		public Transform TargetTransform { get; private set; }
		public IActor CurrentTarget { get; private set; }
		public bool IsTargetNull => TargetTransform == null || CurrentTarget == null;

		private void Awake()
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

			_utilityMachine = _utilities.Build(this);
			_controlled = actor;
			_controlled.OnInit += Initted;
		}

		private void Initted()
		{
			_weapon = (_controlled is IProvider<IWeapon> prov) ? prov.Value : null;
			_vision = GetComponent<AIVision>();
			_vision.OnScan += Scanned;
			_vision.Init(this);
			_utilityMachine.Init(this);
		}

		private void Update()
		{
			_utilityMachine.Update();
		}

		private void Scanned()
		{
			if(_vision.EnemiesInRange != null && _vision.EnemiesInRange.Count > 0 && IsTargetNull)
			{
				CurrentTarget = _vision.EnemiesInRange[0];
				TargetTransform = (CurrentTarget as MonoBehaviour).transform;
				TargetHealth = (CurrentTarget is IProvider<IHealth> prov) ? prov
					.Value : null;
			}
		}

		public bool IsEffectiveToFire(Vector2 point)
		{
			Debug.Log(_weapon);
			Debug.Log((Vector2.Dot(_weapon.LookRotation, Position.GetDirectionNormalized(point)) + 1) / 2 + " " + _fireThreshold / 360);
			return _weapon != null && 
				(point - Position).sqrMagnitude < (_weapon.EffectiveRange * _weapon.EffectiveRange) && 
				(Vector2.Dot(_weapon.LookRotation, Position.GetDirectionNormalized(point)) + 1) / 2 < _fireThreshold / 360; 
			//кто посмеет спросить что тут происходит тот будет уничтожен святым огнем
		}

		public void InitCommand(ControllerAction command)
		{
			OnAction?.Invoke(command);
		}
		public void StopMoving()
		{
			_moveDirection = Vector2.zero;
		}

		public void MoveToPoint(Vector2 point)
		{
			_moveDirection = Position.GetDirectionNormalized(point);
		}

		public void LookAt(Vector2 point)
		{
			_rotation = Position.GetDirectionNormalized(point);
		}

		public IHealth Health => (_controlled is IProvider<IHealth> prov) ? prov.Value : null;
		public IWeapon Weapon => (_controlled is IProvider<IWeapon> prov) ? prov.Value : null;
		public Motor Motor => (_controlled is IProvider<Motor> prov) ? prov.Value : null;
		public AIVision Vision => _vision;
		public Vector2 Position => _controlled.Position;
		public IActor Actor => _controlled;
	}
}