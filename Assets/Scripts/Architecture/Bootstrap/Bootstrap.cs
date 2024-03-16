using UnityEngine;

namespace Architecture
{
	[DefaultExecutionOrder(-10000)]
	[DisallowMultipleComponent()]
	public sealed class Bootstrap : MonoBehaviour
	{
		[SerializeField] private BootstrapElement[] _elements;

		private void Awake()
		{
			Application.targetFrameRate = 360;

			if (_elements == null) Debug.LogError("No elements in bootstraper");

			for (int i = 0; i < _elements.Length; ++i)
			{
				_elements[i].Init();
			}
		}
	}
}
