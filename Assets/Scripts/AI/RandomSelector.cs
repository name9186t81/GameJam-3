using Core;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace AI
{
	public sealed class RandomSelector : IEnemySelector
	{
		public IActor GetEnemy(IEnumerable<IActor> enemies)
		{
			return enemies.ElementAt(Random.Range(0, enemies.Count()));
		}
	}
}
