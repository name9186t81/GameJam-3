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
		private IList<IActor> _cachedScan = new List<IActor>();
		private IList<IActor> _enemies = new List<IActor>();
		private IList<IActor> _alies = new List<IActor>();

		public event Action OnScan;
		public IReadOnlyList<IActor> ActorsInRange => (IReadOnlyList<IActor>)_cachedScan;
		public IReadOnlyList<IActor> EnemiesInRange => (IReadOnlyList<IActor>)_enemies;
		public IReadOnlyList<IActor> AliesInRange => (IReadOnlyList<IActor>)_alies;

		private static Collider2D[] _cachedHits = new Collider2D[128];

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


		public bool CanSeePoint(Vector2 start, Vector2 end)
		{
			return !Physics2D.Linecast(start, end, _blockMask);
		}
		public bool CanSeePoint(Vector2 point)
		{
			//return (!_haveCollider && !Physics2D.Linecast(_controller.Position, point, _wallsMask)) ||
			//		(_haveCollider && Physics2D.LinecastAll(_controller.Position, point, _wallsMask).Length == 1);
			return !Physics2D.Linecast(_controller.Position, point, _blockMask);
		}

		public bool CanSeeTarget(IActor target)
		{
			//return (!_haveCollider && Physics2D.LinecastAll(_controller.Position, target.Position, _wallsMask)[0].transform.TryGetComponent<IActor>(out var act) && act == target) ||
			//		(_haveCollider && Physics2D.LinecastAll(_controller.Position, target.Position, _wallsMask)[1].transform.TryGetComponent<IActor>(out var act2) && act2 == target);
			return CanSeePoint(target.Position);
		}

		public void ForceScan()
		{
			_delay = 0;

			int hits = Physics2D.OverlapCircleNonAlloc(_controller.Position, _range, _cachedHits, _unitsMask);

			IList<IActor> listed = ClearForActors(_cachedHits, hits, _cachedScan);

			_cachedScan = listed;
			if (_controller.Actor is ITeamProvider prov)
			{
				_enemies = ClearForWalls(SortForTeamNumber(prov.TeamNumber, true, _enemies));
				_alies = SortForTeamNumber(prov.TeamNumber, false, _alies);
			}
			OnScan?.Invoke();
		}

		private static Dictionary<Collider2D, IActor> _cachedActors = new Dictionary<Collider2D, IActor>(); //вроде бы это быстрее чем геткомпоненты даже с дип профайлом

		private IList<IActor> ClearForActors(IList<Collider2D> hits, int hitsCount, IList<IActor> cachedList)
		{
			IList<IActor> newList = cachedList;
			cachedList.Clear();

            for (int i = 0; i < hitsCount; i++)
            {
				var col = hits[i];
				bool needToTryGetComponent = true;
				//if (col == null) break; //вернуть если тут будут нулл референс экзепшоны
				if(_cachedActors.TryGetValue(col, out var cachedActor))
                {
					if(cachedActor != null)
                    {
						needToTryGetComponent = false;
						newList.Add(cachedActor);
                    }
                    else
                    {
						_cachedActors.Remove(col);
                    }
                }

				if (needToTryGetComponent && col.transform.TryGetComponent<IActor>(out var act))// && act != null)
				{
					newList.Add(act);
					_cachedActors.Add(col, act);
				}
			}

			return newList;
		}

		private IList<IActor> ClearForWalls(IList<IActor> hits)
		{
            for (int i = 0; i < hits.Count; i++)
            {
				var actor = hits[i];

				if (Physics2D.Linecast(_controller.Position, actor.Position, _blockMask))
				{
					hits.RemoveAt(i);
					i--;
				}
			}
			return hits;
		}

		public IList<IActor> SortForTeamNumber(int teamNumber, bool excluded, IList<IActor> cachedList)
		{
			IList<IActor> list = cachedList;
			cachedList.Clear();

            for (int i = 0; i < _cachedScan.Count; i++)
            {
				var el = _cachedScan[i];
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