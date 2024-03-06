using Core;

using System;
using System.Collections.Generic;
using System.Xml;

using UnityEngine;

namespace AI
{
	[DisallowMultipleComponent()]
	public sealed class AIVision : MonoBehaviour
	{
		[SerializeField] private LayerMask _unitsMask;
		[SerializeField] private LayerMask _blockMask;
		[SerializeField] private float _range;
		[SerializeField] private float _scanFrequancy;
		[SerializeField] private bool _debug;
		private float _delay;
		private AIController _controller;
		private IList<IActor> _cachedScan;
		private IList<IActor> _enemies;
		private IList<IActor> _alies;

		public event Action OnScan;
		public IReadOnlyList<IActor> ActorsInRange => (IReadOnlyList<IActor>)_cachedScan;
		public IReadOnlyList<IActor> EnemiesInRange => (IReadOnlyList<IActor>)_enemies;
		public IReadOnlyList<IActor> AliesInRange => (IReadOnlyList<IActor>)_alies;

		private static Collider2D[] _cachedHits = new Collider2D[64];

		public void Init(AIController controller)
		{
			enabled = true;
			_controller = controller;
		}

		private void Update()
		{
			_delay += Time.deltaTime;
			if (_delay > _scanFrequancy)
			{
				ForceScan();
			}
		}

		public void ForceScan()
		{
			_delay = 0;

			int hits = Physics2D.OverlapCircleNonAlloc(_controller.Position, _range, _cachedHits, _unitsMask);

			IList<IActor> listed = ClearForActors(_cachedHits);
			listed = ClearForWalls(listed);

			_cachedScan = listed;
			if (_controller.Actor is ITeamProvider prov)
			{
				_enemies = SortForTeamNumber(prov.TeamNumber, true);
				_alies = SortForTeamNumber(prov.TeamNumber, false);
			}
			OnScan?.Invoke();
		}


		private IList<IActor> ClearForActors(IList<Collider2D> hits)
		{
			IList<IActor> newList = new List<IActor>();

			foreach (var col in hits)
			{
				if (col == null) break;

				if (col.transform.TryGetComponent<IActor>(out var act) && act.ToString() != "null")
				{
					newList.Add(act);
				}
			}
			return newList;
		}

		private IList<IActor> ClearForWalls(IList<IActor> hits)
		{
			IList<IActor> newList = new List<IActor>();

			foreach (var col in hits)
			{
				if (!Physics2D.Linecast(_controller.Position, col.Position, _blockMask))
				{
					newList.Add(col);
				}
			}
			return newList;
		}

		public IList<IActor> SortForTeamNumber(int teamNumber, bool excluded)
		{
			IList<IActor> list = new List<IActor>();
			foreach (var el in _cachedScan)
			{
				if (!(el is ITeamProvider prov)) continue;

				if (prov.TeamNumber == teamNumber && !excluded || excluded && prov.TeamNumber != teamNumber)
				{
					list.Add(el);
				}
			}
			return list;
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if(!_debug) return;

			Gizmos.DrawWireSphere(transform.position, _range);
		}
#endif
	}
}