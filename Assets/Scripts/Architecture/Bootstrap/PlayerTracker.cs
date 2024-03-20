using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture
{
	[CreateAssetMenu(fileName = "Player tracker", menuName = "Custom/Architecture/PlayerTracker", order = 15)]
	public sealed class PlayerTracker : BootstrapElement
	{
		public override void Init()
		{
			ServiceLocator.Register(new PlayerTrackerService());
		}
	}
}

public class PlayerTrackerService : IService
{
	public PlayerController Player { get; private set; }

	public void Bind(PlayerController player)
	{
		Player = player;
	}
}
