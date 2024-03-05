using UnityEngine;

namespace Movement
{
	public interface IForce
	{
		Vector2 GetForce(Vector2 worldPos);
		ForceState State { get; set; }
	}
}
