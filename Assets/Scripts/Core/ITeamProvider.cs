using System;

namespace Core
{
	public interface ITeamProvider
	{
		int TeamNumber { get; }
		bool TryChangeTeamNumber(int newTeamNumber);
		/// <summary>
		/// Left int is old team number, right is new.
		/// </summary>
		event Action<int, int> OnTeamNumberChange;
	}
}
