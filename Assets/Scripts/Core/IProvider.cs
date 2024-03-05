namespace Core
{
	public interface IProvider<T>
	{
		T Value { get; }
	}
}
