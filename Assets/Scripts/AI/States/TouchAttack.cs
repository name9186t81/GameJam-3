namespace AI.States
{
	 //не юзать нигде кроме классов которые наносят урон по коллизии!!!
	public sealed class TouchAttack : IUtility
	{
		private AIController _controller;

		public void Execute()
		{
			if (_controller.IsTargetNull) return;

			_controller.MoveToPoint(_controller.CurrentTarget.Position);
		}

		public float GetEffectivness()
		{
			return _controller.IsTargetNull ? -100f : 1f;
		}

		public void Init(AIController controller)
		{
			_controller = controller;
		}

		public void PreExecute()
		{
		}

		public void Undo()
		{
			_controller.StopMoving();
		}
	}
}