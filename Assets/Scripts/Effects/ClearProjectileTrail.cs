using UnityEngine;

using Weapons;

namespace Effects
{
	public class ClearProjectileTrail : MonoBehaviour
	{
		[SerializeField] private Projectile _projectile;
		[SerializeField] private TrailRenderer _renderer;

		private void Awake()
		{
			_projectile.OnInit += Clear;
		}

		private void Clear()
		{
			_renderer.Clear();
		}
	}
}