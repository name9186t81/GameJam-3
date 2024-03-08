using UnityEngine;

using Core;

namespace Weapons
{
	public sealed class ImpactVFX : MonoBehaviour
	{
		[SerializeField] private Projectile _projectile;
		[SerializeField] private GameObject _impactSFX;
		[SerializeField] private float _lifeTime;
		private float _elapsed;

		private void Awake()
		{
			_projectile.OnInit += Init;
			_projectile.OnHit += Hit;
		}

		private void Hit(RaycastHit2D arg1, IDamageReactable arg2)
		{
			_impactSFX.SetActive(true);
			_elapsed = 0f;
			_impactSFX.transform.rotation = Quaternion.Euler(0, 0, arg1.normal.AngleFromVector() - 90);
		}

		private void Update()
		{
			_elapsed += Time.deltaTime;
			if (_elapsed > _lifeTime)
			{
				_impactSFX.SetActive(false);
			}
		}

		private void Init()
		{
			_elapsed = 0f;
			_impactSFX.SetActive(false);
		}
	}

}