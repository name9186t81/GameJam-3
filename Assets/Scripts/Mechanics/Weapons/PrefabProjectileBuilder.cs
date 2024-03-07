using System;

using UnityEngine;
using Core;

namespace Weapons
{
	[CreateAssetMenu(fileName = "Projectile Builder", menuName = "GameJam/Projectile Builder", order = 1)]
	public sealed class PrefabProjectileBuilder : ProjectileBuilder
	{
		[SerializeField] private GameObject _projectile;

		public override IProjectile BuildProjectile(IActor owner)
		{
			GameObject instance = MonoBehaviour.Instantiate(_projectile);
			if (!instance.TryGetComponent<IProjectile>(out var projectile))
			{
				throw new ArgumentException();
			}
			return projectile;
		}

		public override IProjectile PeekProjectile()
		{
			return _projectile.GetComponent<IProjectile>();
		}
	}
}