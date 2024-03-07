namespace AI
{
	public sealed class UtilityMachine
	{
		private readonly IUtility[] _utilities;
		private IUtility _current;

		public UtilityMachine(IUtility[] utilities)
		{
			_utilities = utilities;
		}

		public void Init(AIController controller)
		{
			foreach(var utility in _utilities)
			{
				utility.Init(controller);
			}
		}
		public void Update()
		{
			IUtility utility = _utilities[0];
			float max = utility.GetEffectivness();

			for (int i = 1, length = _utilities.Length; i < length; ++i)
			{
				float effect = _utilities[i].GetEffectivness();
				if(effect > max)
				{
					max = effect;
					utility = _utilities[i];
				}
			}

			if(utility != _current)
			{
				_current?.Undo();
				utility.PreExecute();
				_current = utility;
			}

			_current.Execute();
		}
	}
}