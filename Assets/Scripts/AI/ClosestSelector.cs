using Core;

using System.Collections.Generic;

namespace AI
{
	public sealed class ClosestSelector : IEnemySelector
	{
		private readonly IActor _checked;

		public ClosestSelector(IActor @checked)
		{
			_checked = @checked;
		}

		public IActor GetEnemy(IEnumerable<IActor> enemies)
		{
			IActor selected = null;
			float dist = float.MaxValue;

			foreach(var enemy in enemies)
			{
				float currentDistance = enemy.Position.GetDirection(_checked.Position).sqrMagnitude;
				if(currentDistance < dist)
				{
					dist = currentDistance;
					selected = enemy;
				}
			}
			return selected;
		}
	}
}
