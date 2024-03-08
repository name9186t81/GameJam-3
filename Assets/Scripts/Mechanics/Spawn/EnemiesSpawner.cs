using Core;

using GameLogic;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Spawning
{
	public class EnemiesSpawner : MonoBehaviour
	{
		[Serializable]
		private class SpawnInfo
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
		private int _totalSpawned;

		private void Start()
		{
			if ((_playerActor = FindObjectOfType<PlayerActor>()) == null)
			{
				Debug.LogError("Cannot find player actor");
				Destroy(gameObject);
				return;
				//умоляю сделайте сервис с игроком
			}

			ServiceLocator.Get<GlobalDeathNotificator>().OnDeath += Death;
		}

		private void Death(Health.DamageArgs arg1, IActor arg2)
		{
			for(int i = 0; i < _spawns.Length; i++)
			{
				Debug.Log(_spawns[i].EnemyPrefab.Name + " " + arg2.Name);
				if (_spawns[i].EnemyPrefab.Name == arg2.Name)
				{
					_spawns[i].Spawned--;
					_totalSpawned--;
				}
			}
		}

		private void Update()
		{
			_elapsed += Time.deltaTime;
			_totalTime += Time.deltaTime;

			if (_elapsed > _spawnDelay)
			{
				_elapsed = 0;
				Spawn();
			}
		}

		private void OnDestroy()
		{
			ServiceLocator.Get<GlobalDeathNotificator>().OnDeath -= Death;
		}

		private void Spawn()
		{
			if (_totalSpawned > _maxSpawns) return;
			var unit = FindBest();
			var radomPoint = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Length)];

			_totalSpawned++;
			var obj = Instantiate(unit, radomPoint.Position, Quaternion.identity, null);
		}

		private Unit FindBest()
		{
			float score = _playerActor.CurrentScore;
			float totalFactor = 0f;
			List<(SpawnInfo, float)> choosen = new List<(SpawnInfo, float)>();

			for(int i = 0; i < _spawns.Length; i++)
			{
				if (_spawns[i].MiniumScore > score || _spawns[i].Spawned >= _spawns[i].MaxSpawns) continue;

				float timeFactor = _spawns[i].ChanceOverTime.Evaluate(MathF.Min(_totalTime / _spawns[i].MaxTime, 1f));
				_totalTime += timeFactor * _spawns[i].SpawnMultiplayer;
				choosen.Add((_spawns[i], totalFactor));
			}

			float acFactor = 0f;
			float random = UnityEngine.Random.Range(0, totalFactor);
			for(int i = 0; i < choosen.Count; i++)
			{
				if(random < choosen[i].Item2)
				{
					choosen[i].Item1.Spawned++;
					return choosen[i].Item1.EnemyPrefab;
				}

				acFactor += choosen[i].Item2;
			}

			return _spawns[0].EnemyPrefab;
		}
	}
}