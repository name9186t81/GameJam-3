using UnityEngine;

namespace Abilities
{
	public interface IDirectionalAbility : IAbility
	{
		Vector2 Direction { set; }
	}
}