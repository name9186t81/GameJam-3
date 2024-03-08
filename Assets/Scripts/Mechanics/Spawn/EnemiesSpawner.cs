using Core;

using GameLogic;

using System;

using UnityEngine;

namespace Spawning
{
	public class EnemiesSpawner : MonoBehaviour
	{
		[Serializable]
		private struct SpawnInfo
		{
			public Unit EnemyPrefab;
			[Range(0, 1f)] public float SpawnMultiplayer;
			public AnimationCurve ChanceOverTime;
			public float MaxTime;
			public int MiniumScore;
			public int MaxSpawns;

			[HideInInspector] public int Spawned;
		}

		[SerializeField] private int _maxSpawns;
		[SerializeField] private float _spawnDelay;
		[SerializeField] private SpawnPoint[] _spawnPoints;
		[SerializeField] private SpawnInfo[] _spawns;
		private PlayerActor _playerActor;
		private float _elapsed;
		private float _totalTime;

		private void Start()
		{
			if ((_playerActor = FindObjectOfType<PlayerActor>()) == null)
			{
				Debug.LogError("Cannot find player actor");
				Destroy(gameObject);
				return;
				//умоляю сделайте сервис с игроком
			}
		}

		private void Update()
		{
			_elapsed += Time.deltaTime;

			if (_elapsed > _spawnDelay)
			{
				_totalTime += _elapsed;
				_elapsed = 0;
				Spawn();
			}
		}

		private void Spawn()
		{
			var unit = FindBest();
		}

		private Unit FindBest()
		{
			return null;
			float score = _playerActor.CurrentScore;

			for(int i = 0; i < _spawns.Length; i++)
			{
				if (_spawns[i].MiniumScore > score) continue;
			}
		}
	}
}