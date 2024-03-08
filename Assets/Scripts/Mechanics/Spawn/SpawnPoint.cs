using UnityEngine;

namespace Spawning
{
	public sealed class SpawnPoint : MonoBehaviour
	{
		public Vector2 Position => transform.position;
		public bool IsAvaible { get
			{
				var point = Camera.main.WorldToViewportPoint(Position);
				return (point.x < 0 || point.x > 1) || (point.y < 0 || point.y > 1);
			} }
	}
}