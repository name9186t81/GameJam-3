using UnityEngine;

namespace Spawning
{
	public sealed class SpawnPoint : MonoBehaviour
	{
		public Vector2 Position => transform.position;
	}
}