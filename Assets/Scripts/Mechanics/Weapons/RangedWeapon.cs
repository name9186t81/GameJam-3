using Core;

using Health;

using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

namespace Weapons
{
	[DisallowMultipleComponent()]
	public sealed class RangedWeapon : MonoBehaviour, IWeapon
	{
		//почему тип разброса здесь а фабрика в другом месте? а спросите того кто пишет свойства с маленькой буквы
		public enum SpreadType
		{
			Fixed,
			Angle
		}
		[SerializeField] private SpreadType _spreadType;
		[SerializeField] private int _spreadIterations;
		[SerializeField] private int _spreadOffset;
		[SerializeField] private float _spreadAngle;
		[SerializeField] private WeaponFlags _flags;
		[SerializeField] private DamageFlags _damageFlags;
		[SerializeField] private ProjectileBuilder _projectileBuilder;
		[SerializeField] private Vector2 _shootPoint;
		[SerializeField] private int _baseDamage;
		[SerializeField] private int _projectileCount = 1;
		[SerializeField] private float _preFireDelay;
		[SerializeField] private float _delayPerProjectile;
		[SerializeField] private float _postFireDelay;
		[SerializeField] private float _effectiveRange;
		[SerializeField] private float _useRange;
		[SerializeField] private bool _rotatble;
		[SerializeField] private bool _debug;

		private Transform _transform;
		private bool _canFire = true;
		private bool _isFiring = false;
		private bool _isInited = false;
		private Pool<IProjectile> _projectilePool;
		private IActor _actor;
		private ISpreadProvider _spread;
		public bool CanAttack => !_isFiring && _canFire;

		public float EffectiveRange => _effectiveRange * 3; // эти баланс правки просто имба (я не виноват что еда сделал оружие где то там внутри иерархии и мне лень по каждому бегать и увеличивать)

		public float UseRange => _useRange;

		public WeaponFlags Flags { get => _flags; set => _flags = value; }

		public bool IsInited => _isInited;

		public bool Rotatable => _rotatble;

		public Vector2 LookRotation { get => _transform.up; set => _transform.up = value; }

		public event Action OnInit;
		public event Action OnAttack;
		public event Action OnAttackEnded;

		public event Action OnPreFireStart; //а это для реальной логики потому что юнити "евенты" сосут
		public event Action OnPreFireEnd;
		public event Action OnFire;
		public event Action OnEndFire;

		public UnityEvent PreFireStart; //они для визуала потому что мне лень писать отдельные классы для этого
		public UnityEvent PreFireEnd;
		public UnityEvent Fire;
		public UnityEvent EndFire;

		private void Start()
		{
			_transform = transform;
			_projectilePool = new Pool<IProjectile>(() => _projectileBuilder.BuildProjectile(_actor));

			OnPreFireStart += () => PreFireStart?.Invoke();
			OnFire += () => Fire?.Invoke();
			OnPreFireEnd += () => PreFireEnd?.Invoke();
			OnEndFire += () => EndFire?.Invoke();

			_spread = SpreadFactory.GetSpread(_spreadType, _spreadIterations, _spreadAngle * Mathf.Deg2Rad, _spreadOffset);
		}

		private void OnDestroy()
		{
			_projectilePool.Destroy();
		}

		private IEnumerator FireRoutine()
		{
			_isFiring = true;
			OnPreFireStart?.Invoke();
			yield return new WaitForSeconds(_preFireDelay); //закешировать сранные вейт фор секонды если лагать будет
			OnPreFireEnd?.Invoke();

			if ((_flags & WeaponFlags.Freezed) != 0)
				yield break;

			for (int i = 0; i < _projectileCount; i++)
			{
				var obj = _projectilePool.Get();
				var args = new DamageArgs(_actor, (int)(_baseDamage), _damageFlags | DamageFlags.Ranged, this); //форсирование рейнджеда в другом месте должно быть
				obj.Init(_projectilePool, args, (_actor is ITeamProvider prov) ? prov.TeamNumber : 0, GlobalShootPoint, _spread.GetDirection(ShootDirection), _actor, 1f);
				OnFire?.Invoke();
				OnAttack?.Invoke();
				if ((_flags & WeaponFlags.Freezed) != 0)
					yield break;
				if (_delayPerProjectile != 0)
					yield return new WaitForSeconds(_delayPerProjectile);
			}

			OnAttackEnded?.Invoke();
			yield return new WaitForSeconds(_postFireDelay);
			_isFiring = false;
		}

		public void Init(IActor actor)
		{
			_actor = actor;
			_transform = transform;
			_actor.OnAction += ReadAction;

			_isInited = true;
			OnInit?.Invoke();
		}

		private void ReadAction(ControllerAction obj)
		{
			if ((_flags & WeaponFlags.Freezed) != 0)
				return;

			if (obj == ControllerAction.Fire && CanAttack) StartCoroutine(FireRoutine());
		}


#if UNITY_EDITOR //эта валидация отстой ну я всеравно оружие не делаю вне инспектора
		private void OnValidate()
		{
			_flags |= WeaponFlags.Ranged;

			_projectileCount = Mathf.Max(_projectileCount, 0);
			_preFireDelay = Mathf.Max(_preFireDelay, 0);
			_delayPerProjectile = Mathf.Max(_delayPerProjectile, 0);
			_postFireDelay = Mathf.Max(_postFireDelay, 0);
			_effectiveRange = Mathf.Max(_effectiveRange, 0);
			_useRange = Mathf.Max(_useRange, 0);
			_baseDamage = Mathf.Max(_baseDamage, 0);
		}
		private void OnDrawGizmos()
		{
			if (!_debug) return;

			Vector2 selfPos = transform.position;
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(selfPos, selfPos + _shootPoint.Rotate(transform.eulerAngles.z * Mathf.Deg2Rad));
			Gizmos.DrawWireSphere(selfPos + _shootPoint, 0.1f);

			Gizmos.DrawWireSphere(selfPos, _useRange);
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(selfPos, _effectiveRange);
		}
#endif
		public Vector2 ShootDirection => _transform.up;
		public Vector2 GlobalShootPoint => (Vector2)_transform.position + _shootPoint.Rotate(transform.eulerAngles.z * Mathf.Deg2Rad);

	}
}
