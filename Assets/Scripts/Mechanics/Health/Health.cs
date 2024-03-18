using System;

using UnityEngine;
using Core;
using static Health.IHealth;

namespace Health
{
	public class Health : IHealth
	{
		private readonly int _maxHealth;
		private readonly ITeamProvider _selfTeam;
		private HealthFlags _flags;
		private int _currentHealth;
		private IActor _owner;

		public Health(int maxHealth, int currentHealth, ITeamProvider prov = null)
		{
			_maxHealth = maxHealth;
			_currentHealth = currentHealth;
			_flags = HealthFlags.FriendlyFireDisabled;
			_selfTeam = prov;

			_owner = null;
			OnDamage = default;
			OnDeath = default;
		}

		public int CurrentHealth => _currentHealth;

		public int MaxHealth => _maxHealth;

		public HealthFlags Flags { get => _flags; set => _flags = value; }
		public IActor Actor { set => _owner = value; get => _owner; }

		public event Action<DamageArgs> OnDamage;
		public event Action<DamageArgs> OnDeath;

		public bool CanTakeDamage(DamageArgs args)
		{
			if (_selfTeam != null && (Flags & HealthFlags.FriendlyFireDisabled) != 0)
			{
				return args.Sender == null || (args.Sender != null && args.Sender is ITeamProvider team && team.TeamNumber != _selfTeam.TeamNumber);
			}
			return (Flags & HealthFlags.Invincible) == 0;
		}

		public void TakeDamage(DamageArgs args)
		{
			//if (args.Sender != null && args.Sender is ITeamProvider prov && prov.TeamNumber == _selfTeam?.TeamNumber && (args.DamageFlags & DamageFlags.Heal) == 0) return;

			if ((Flags & HealthFlags.Invincible) != 0) return;

			if ((args.DamageFlags & DamageFlags.Heal) != 0)
			{
				_currentHealth = Mathf.Min(_currentHealth + args.Damage, _maxHealth);
			}
			else
			{
				_currentHealth -= args.Damage;
			}

			if (_currentHealth <= 0)
			{
				OnDeath?.Invoke(args);
				if (ServiceLocator.TryGet<GlobalDeathNotificator>(out var notif))
				{
					notif.Die(args, _owner);
				}
				return;
			}

			OnDamage?.Invoke(args);
		}
	}
}
