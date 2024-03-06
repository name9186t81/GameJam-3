namespace AI
{
	public interface IUtility
	{
		float GetEffectivness();
		void Init(AIController controller);
		void Execute();
		void PreExecute();
		void Undo();
	}
}