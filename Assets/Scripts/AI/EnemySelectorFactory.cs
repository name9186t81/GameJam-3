using Core;

using System;

namespace AI
{
	public static class EnemySelectorFactory
	{
		public enum Type
		{
			Strongest,
			Closest,
			Random
		}

		public static IEnemySelector GetSelector(IActor owner, Type type)
		{
			switch(type)
			{
				case Type.Random: return new RandomSelector();
				case Type.Strongest: return new StrongestSelector();
				case Type.Closest: return new ClosestSelector(owner);
				default: throw new NotImplementedException();
			}
		}
	}
}
