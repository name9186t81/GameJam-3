using Health;

using Movement;

using System;

using UnityEngine;

using Weapons;

namespace Core
{
	[DisallowMultipleComponent(), RequireComponent(typeof(Rigidbody2D))]
	public sealed class Unit : MonoBehaviour, 
		IActor, IProvider<Motor>, IProvider<IHealth>, 
		IProvider<IWeapon>, IMovable, ITeamProvider,
		IDamageReactable
	{
		[Header("Motor settings")]
		[SerializeField] private float _speed;
		[SerializeField] private float _rotationSpeed;
		private Motor _motor;

		[Space]
		[Header("Health settings")]
		[SerializeField] private int _healthAmount;
		private IHealth _health;

		[Space]
		[Header("Team settings")]
		[SerializeField] private int _teamNumber;
		[SerializeField] private bool _canChangeTeams;

		private Transform _transform;
		private IController _controller;
		private IWeapon _weapon;
		private Rigidbody2D _rigidbody;
		public Vector2 Position => _transform.position;

		public IController Controller => _controller;

		public Motor Value => _motor;

		IHealth IProvider<IHealth>.Value => _health;

		IWeapon IProvider<IWeapon>.Value => _weapon;

		public float Rotation { get => _transform.eulerAngles.z; set => _transform.rotation = Quaternion.Euler(0, 0, value); }
		public Vector2 Velocity { get => _rigidbody.velocity; set => _rigidbody.velocity = value; }

		public int TeamNumber => _teamNumber;

		public event Action<ControllerAction> OnAction;
		public event Action OnInit;
		public event Action<int, int> OnTeamNumberChange;
		public event Action<DamageArgs> OnDamage;

		private void Start()
		{
			_transform = transform;
			_rigidbody = GetComponent<Rigidbody2D>();
			_motor = new Motor(_speed, _rotationSpeed, this, this);
			_health = new Health.Health(_healthAmount, _healthAmount);
			_weapon = GetComponentInChildren<IWeapon>();
			if(_weapon == null)
			{
				Debug.LogError("Unit does not have weapon");
			}
			_weapon.Init(this);
			_health.OnDeath += (_) => Destroy(gameObject);
			_health.Flags |= HealthFlags.FriendlyFireDisabled;
			OnInit?.Invoke();
		}
		private void Update()
		{
			_motor.Update(Time.deltaTime);
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

		public bool TryChangeTeamNumber(int newTeamNumber)
		{
			if(!_canChangeTeams) return false;
			OnTeamNumberChange?.Invoke(_teamNumber, newTeamNumber);
			_teamNumber = newTeamNumber;
			return true;
		}

		public bool CanTakeDamage(DamageArgs args)
		{
			return _health.CanTakeDamage(args);
		}

		public void TakeDamage(DamageArgs args)
		{
			_health.TakeDamage(args);
			OnDamage?.Invoke(args);
		}
	}
}