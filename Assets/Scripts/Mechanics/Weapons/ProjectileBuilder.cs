using UnityEngine;
using Core;

namespace Weapons
{
	public abstract class ProjectileBuilder : ScriptableObject
	{
		public abstract IProjectile PeekProjectile();
		public abstract IProjectile BuildProjectile(IActor owner);
	}
}
