using UnityEngine;

namespace Architecture
{
	[DisallowMultipleComponent()]
	public sealed class Bootstrap : MonoBehaviour
	{
		[SerializeField] private BootstrapElement[] _elements;

		private void Awake()
		{
			if (_elements == null) Debug.LogError("No elements in bootstraper");

			for (int i = 0; i < _elements.Length; ++i)
			{
				_elements[i].Init();
			}
		}
	}
}
