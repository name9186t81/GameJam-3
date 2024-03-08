using System;
using System.Collections;

using UnityEngine;
using System.Collections.Generic;
using Core;
using Health;

namespace Weapons
{
	[DisallowMultipleComponent()]
	public sealed class Projectile : MonoBehaviour, IProjectile, ITeamProvider
	{
		[SerializeField] private float _speed;
		[SerializeField] private float _delayBeforeDestroy;
		[SerializeField] private LayerMask _hitMask;
		[SerializeField] private ProjectileFlags _flags;
		[SerializeField] private bool _allowForcedDirectionChanges;
		[SerializeField] private float _lifeTime;

		private float _timeMod = 1f;
		private WaitForSeconds _delay;
		private float _elapsed;
		private IActor _owner;
		private Vector2 _direction;
		private Vector2 _positionInlastFrame;
		private Vector2 _startPosition;
		private Transform _cached;
		private Pool<IProjectile> _pool;
		private DamageArgs _args;
		private float _speedModifier;
		private int _teamNumber;
		private bool _markedForDestroy;
		private HashSet<Collider2D> _hitted;

		public event Action<RaycastHit2D, IDamageReactable> OnHit;
		public event Action<int, int> OnTeamNumberChange;
		public event Action<IActor> OnParry;
		public event Action OnInit;

		public DamageArgs Args => _args;

		public float Speed => _speed;

		public IActor Source => _owner;

		public Vector2 Direction => _direction;

		public ProjectileFlags ProjectileFlags => _flags;

		public int TeamNumber => _teamNumber;

		public Vector2 Position => _cached.position;

		public Vector2 StartPosition => _startPosition;

		private void Awake()
		{
			_cached = transform;
			_delay = new WaitForSeconds(_delayBeforeDestroy);
		}

		public void Init(Pool<IProjectile> pool, DamageArgs args, int teamNumber, in Vector2 startPos, in Vector2 direction, IActor owner, float speedModifier)
		{
			_flags &= ~ProjectileFlags.Frozen;
			_pool = pool;
			_owner = owner;
			_direction = direction;
			_positionInlastFrame = _startPosition = _cached.position = startPos;
			_startPosition = owner == null ? _startPosition : _owner.Position;
			_args = args;
			_speedModifier = speedModifier;
			OnTeamNumberChange?.Invoke(_teamNumber, teamNumber);
			_teamNumber = teamNumber;
			_elapsed = 0f;
			gameObject.SetActive(true);
			_cached.rotation = Quaternion.Euler(0, 0, direction.AngleFromVector() - 90);
			if (_pool != null)
			{
				_pool.OnDestroy -= MarkForDestroy;
			}
			_pool.OnDestroy += MarkForDestroy;
			_hitted = new HashSet<Collider2D>();
			OnInit?.Invoke();
		}

		private void MarkForDestroy()
		{
			//удачи как понять что объект уже мертв
			if (this != null && !_markedForDestroy && gameObject != null && !gameObject.activeSelf) Destroy(gameObject);
			_markedForDestroy = true;
		}

		public bool TryChangeDirection(in Vector2 newDirection)
		{
			if (_allowForcedDirectionChanges)
			{
				_direction = newDirection;
			}

			return _allowForcedDirectionChanges;
		}

		private void Update()
		{
			if ((_flags & ProjectileFlags.Frozen) != 0)
			{
				return;
			}

			_elapsed += Time.deltaTime * _timeMod;
			if (_elapsed > _lifeTime)
			{
				StartCoroutine(DelayBeforeReturn());
			}
			_positionInlastFrame = _cached.position;
		}

		private void LateUpdate()
		{
			if ((_flags & ProjectileFlags.Frozen) != 0)
			{
				return;
			}

			_cached.position = _positionInlastFrame + Direction * Time.deltaTime * _speed * _speedModifier * _timeMod;
			Vector2 currentPosition = _cached.position;

			float length = Vector2.Distance(_positionInlastFrame, currentPosition);

			var raycast = Physics2D.Raycast(_positionInlastFrame, (currentPosition - _positionInlastFrame) / length, length, _hitMask);
			if (raycast && !raycast.collider.isTrigger)
			{
				bool hittedCheck = !_hitted.Contains(raycast.collider);
				bool invoked = false;
				IDamageReactable act = null;
				if (raycast.transform.TryGetComponent<IDamageReactable>(out act) && hittedCheck)
				{
					bool check = act.CanTakeDamage(_args);
					if (check)
					{
						_args.HitPosition = raycast.point;
						_args.SourcePosition = _startPosition;
						act.TakeDamage(_args);
						invoked = true;
						OnHit?.Invoke(raycast, act);
					}
					_hitted.Add(raycast.collider);
				}
				if (((_flags & ProjectileFlags.GoThroughTargets) == 0) && hittedCheck)
				{
					_cached.position = raycast.point;
					if (!invoked) OnHit?.Invoke(raycast, act);
					StartCoroutine(DelayBeforeReturn());
				}
			}
		}

		private IEnumerator DelayBeforeReturn()
		{
			_flags |= ProjectileFlags.Frozen;
			yield return _delay;
			if (_markedForDestroy)
			{
				Destroy(gameObject);
			}
			_pool?.ReturnToPool(this);
			gameObject.SetActive(false);
		}

		public bool TryChangeTeamNumber(int newTeamNumber)
		{
			OnTeamNumberChange?.Invoke(TeamNumber, newTeamNumber);
			_teamNumber = newTeamNumber;
			return true;
		}

		public void Destroy()
		{
			if (isActiveAndEnabled)
				StartCoroutine(DelayBeforeReturn());
		}
	}
}