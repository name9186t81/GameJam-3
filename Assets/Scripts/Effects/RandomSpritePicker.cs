using UnityEngine;

namespace Effects
{
	public sealed class RandomSpritePicker : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer _renderer;
		[SerializeField] private Sprite[] _variants;

		private void Start()
		{
			_renderer.sprite = _variants[Random.Range(0, _variants.Length)];
		}
	}
}