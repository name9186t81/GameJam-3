using UnityEngine;

namespace AI.States
{
	public sealed class ReturnToOwner : IUtility
	{
		private readonly float _timeToReturn;
		private AIController _controller;
		private float _elapsed;

		public ReturnToOwner(float timeToReturn)
		{
			_timeToReturn = timeToReturn;
		}

		public void Execute()
		{
			_controller.MoveToPoint(_controller.Owner.Position);
		}

		public float GetEffectivness()
		{
			_elapsed += Time.deltaTime;
			return  _controller.Owner != null ? _elapsed / _timeToReturn : -1f;
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
