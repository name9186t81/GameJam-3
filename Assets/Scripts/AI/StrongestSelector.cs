using Core;

using Health;

using System.Collections.Generic;

using UnityEngine;

namespace AI
{
	public sealed class StrongestSelector : IEnemySelector
	{
		public IActor GetEnemy(IEnumerable<IActor> enemies)
		{
			IActor selected = null;
			float strength = float.MinValue;

			foreach(var enemy in enemies)
			{
				Debug.Log(enemy + " " + (enemy is IProvider<IHealth> prov2 ? (prov2.Value.MaxHealth + " " + prov2) : "") + " " + strength);
				if (enemy is IProvider<IHealth> prov && prov.Value.MaxHealth > strength)
				{
					selected = enemy;
					strength = prov.Value.MaxHealth;
				}
			}

			return selected;
		}
	}
}
