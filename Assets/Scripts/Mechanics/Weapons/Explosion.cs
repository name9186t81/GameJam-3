using Core;
using Health;
using Movement;

using UnityEngine;
using UnityEngine.Events;

namespace Weapons
{
	public sealed class Explosion : MonoBehaviour
	{
		[SerializeField] private Projectile _projectile;
		[SerializeField] private float _radius;
		[SerializeField] private int _damage;
		[SerializeField] private DamageFlags _flags;
		[SerializeField] private LayerMask _targetsMask;
		[SerializeField] private LayerMask _blockMask;
		[SerializeField] private float _pushForce;
		[SerializeField] private float _pushMultiplayerForOwner;
		[SerializeField] private float _pushTime;

		[SerializeField] private bool _pushTargets;
		[SerializeField] private bool _ignoreRestrictions;
		[SerializeField] private bool _affectProjectiles;
		[SerializeField] private bool _changeProjectileTeam;
		[SerializeField] private bool _debug;

		public IDamageReactable Owner;
		public UnityEvent OnExplode;
		private static Collider2D[] _cachedResults;

		static Explosion()
		{
			_cachedResults = new Collider2D[64];
		}

		private void Awake()
		{
			//todo: weird logic, make separate class for projectile interaction
			if (_projectile != null)
				_projectile.OnHit += Hit;
		}

		private void Hit(RaycastHit2D arg1, IDamageReactable arg2)
		{
			if (_projectile.Source is IDamageReactable react)
			{
				Owner = react;
			}
			Explode(arg1.point - _projectile.Direction * 0.05f, _projectile.Source, _projectile.TeamNumber);
		}

		public void Explode(in Vector2 center, IActor sender, int teamNumber)
		{
			int targetsCount = Physics2D.OverlapCircleNonAlloc(center, _radius, _cachedResults, _targetsMask);

			DamageArgs newArgs = new DamageArgs(sender, _damage, _flags, null);
			for (int i = 0; i < targetsCount; ++i)
			{
				var point = _cachedResults[i].ClosestPoint(center);
				if (Physics2D.Linecast(center, point, _blockMask)) continue;

				if (_cachedResults[i].TryGetComponent<IDamageReactable>(out var react))
				{
					newArgs.HitPosition = point;
					if (_ignoreRestrictions || react.CanTakeDamage(newArgs))
						react.TakeDamage(newArgs);
				}

				if (_affectProjectiles && _cachedResults[i].TryGetComponent<IProjectile>(out var proj))
				{
					proj.TryChangeDirection(center.GetDirectionNormalized(point));
					if (_changeProjectileTeam && proj is ITeamProvider prov2)
					{
						prov2.TryChangeTeamNumber(teamNumber);
					}
				}

				if (_pushTargets && _cachedResults[i].TryGetComponent<IProvider<Motor>>(out var prov))
				{
					var force = ForceFactory.GetParameterizedForce();
					if (react == Owner)
					{
						force[ParameterizedForceKey.Strength] = _pushForce * _pushMultiplayerForOwner;
					}
					else
					{
						force[ParameterizedForceKey.Strength] = _pushForce;
					}
					force[ParameterizedForceKey.PushDirection] = center.GetDirectionNormalized(point);
					force[ParameterizedForceKey.ElapsedTime] = force[ParameterizedForceKey.MaxTime] = _pushTime;
					force[ParameterizedForceKey.Motor] = prov.Value;

					force.SetForce((_) =>
					{
						float elapsed = (float)force[ParameterizedForceKey.ElapsedTime];
						float max = (float)force[ParameterizedForceKey.MaxTime];
						elapsed -= Time.deltaTime;

						if (elapsed < 0)
						{
							((Motor)force[ParameterizedForceKey.Motor]).RemoveForce(force);
							return Vector2.zero;
						}

						float delta = elapsed / max;
						force[ParameterizedForceKey.ElapsedTime] = elapsed;
						return (Vector2)force[ParameterizedForceKey.PushDirection] * (float)force[ParameterizedForceKey.Strength] * delta;
					});
					prov.Value.AddForce(force);
				}
			}
			OnExplode?.Invoke();
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (!_debug) return;

			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, _radius);
		}
#endif
	}
}
