using Core;

using System.Collections.Generic;

namespace AI
{
	public interface IEnemySelector
	{
		IActor GetEnemy(IEnumerable<IActor> enemies);
	}
}
