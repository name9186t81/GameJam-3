namespace Weapons
{
	internal class SpreadFactory
	{
		//зачем вообще фабрика если тут 1 условие просто? затем же зачем я и пишу коммы в 2 ночи
		public static ISpreadProvider GetSpread(RangedWeapon.SpreadType spreadType, int iterations, float angle, int offset = 0)
		{
			if (spreadType == RangedWeapon.SpreadType.Fixed) return new FixedSpread();
			return new AngleSpread(iterations, angle, offset);
		}

	}
}
