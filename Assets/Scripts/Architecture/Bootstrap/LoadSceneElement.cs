using UnityEngine;
using UnityEngine.SceneManagement;

namespace Architecture
{
	[CreateAssetMenu(fileName = "Load scene element", menuName = "Custom/Architecture/LoadSceneNode", order = 10)]
	public sealed class LoadSceneElement : BootstrapElement
	{
		[SerializeField] private string _sceneName;

		public override void Init()
		{
			SceneManager.LoadScene(_sceneName);
		}
	}
}
