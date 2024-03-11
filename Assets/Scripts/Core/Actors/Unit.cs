using Health;

using Movement;

using System;
using System.Collections;
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
		[SerializeField] private float _scale = 0.5f;
		[SerializeField] private float _rotationSpeed;
		[SerializeField] private bool _faceDirectionToMove;
		private Motor _motor;

		[Space]
		[Header("Health settings")]
		[SerializeField] private int _healthAmount;
		private IHealth _health;

		[Space]
		[Header("Team settings")]
		[SerializeField] private int _teamNumber;
		[SerializeField] private bool _canChangeTeams;
		[SerializeField] private string _name;

		private Transform _transform;
		private IController _controller;
		private IWeapon _weapon;
		private Rigidbody2D _rigidbody;
		public Vector2 Position { get => _transform.position; set { _transform.position = value; } }

		public IController Controller => _controller;

		public Motor Value => _motor;

		IHealth IProvider<IHealth>.Value => _health;

		IWeapon IProvider<IWeapon>.Value => _weapon;

		public float Rotation { get => _transform.eulerAngles.z; set => _transform.rotation = Quaternion.Euler(0, 0, value); }
		public Vector2 Velocity { get => _rigidbody.velocity; set => _rigidbody.velocity = value; }

		public int TeamNumber => _teamNumber;

		public float Scale => _scale;
		public float Radius => Scale;

		public event Action<ControllerAction> OnAction;
		public event Action OnInit;
		public event Action<int, int> OnTeamNumberChange;
		public event Action<DamageArgs> OnDamage;

		private bool _freezed = false;

		private void Awake()
		{
			_transform = transform;
		}

		private void Start()
		{
			_transform = transform;
			_rigidbody = GetComponent<Rigidbody2D>();
			_rigidbody.constraints |= RigidbodyConstraints2D.FreezeRotation;
			_motor = new Motor(_speed, _rotationSpeed, this, this, _faceDirectionToMove);
			_health = new Health.Health(_healthAmount, _healthAmount, this);
			_health.Actor = this;
			_weapon = GetComponentInChildren<IWeapon>();
			if(_weapon == null)
			{
				Debug.LogError("Unit does not have weapon");
			}
			_weapon.Init(this);
			_health.OnDeath += OnDeath;
			_health.Flags |= HealthFlags.FriendlyFireDisabled;
			OnInit?.Invoke();
		}

		private void OnDeath(DamageArgs args)
        {
			_freezed = true;
			if (_weapon != null)
				_weapon.Flags |= WeaponFlags.Freezed;

			var colls = gameObject.GetComponentsInChildren<Collider2D>();
			foreach(var coll in colls)
            {
				coll.enabled = false;
            }
			StartCoroutine(DeathCor(args, GetComponent<SpriteRenderer>()));
		}

		private const float _deathEffectTime = 1f;
		private const float _deathLerpTime = 0.2f;
		private IEnumerator DeathCor(DamageArgs args, SpriteRenderer renderer)
        {
			var startTime = Time.time;

			if (renderer)
				renderer.sortingOrder = 20; //у игрока 15 тип да чтобы поверх игрока да согласен говно

			var startPos = Position;

			//var offset = UnityEngine.Random.insideUnitCircle * args.Radius * 0.5f;
			var offset = Vector2.ClampMagnitude((Position - args.Sender.Position) / args.Radius, 1) * args.Radius * 0.5f;

			while (Time.time - startTime < _deathEffectTime && args.Sender != null)
            {
				var prog = (Time.time - startTime) / _deathEffectTime;
				var progLerp = Mathf.Clamp01((Time.time - startTime) / _deathLerpTime);

				if (renderer)
					renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1 - prog);

				Position = Vector3.Lerp(startPos, args.Sender.Position + offset, progLerp);

				yield return null; //сделал лерп по другому
				//yield return new WaitForFixedUpdate(); // фиксед потому что лерп да говно
            }

			yield return null;
			Destroy(gameObject);
		}

		private void Update()
		{
			if(!_freezed)
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
			if (!_freezed)
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

		public string Name => _name;
	}
}