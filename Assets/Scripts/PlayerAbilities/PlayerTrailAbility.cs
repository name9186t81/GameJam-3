using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAbilities
{
    public class PlayerTrailAbility : AbilitiesContainer.Ability
    {
        [SerializeField] private float _workingTime;
        [SerializeField] private float _triggerDestroyTime;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField][Range(0, 2)] private float _triggerSpawnDistanceRadiusMult = 1;
        [SerializeField] private DamagingTrigger _triggerPrefab;

        private float _startTime;
        private bool _working;
        private Vector2 _lastTriggerSpawnPoint;

        private void Start()
        {
            _trailRenderer.time = _triggerDestroyTime;
        }

        private protected override void update()
        {
            if(!_working)
            {
                if (TryUse(false))
                {
                    _working = true;
                    _trailRenderer.transform.position = _player.BonesConnector.TransformPosition;
                    _trailRenderer.Clear();
                    _lastTriggerSpawnPoint = Vector2.one * float.MaxValue;
                    _startTime = Time.time;
                }
            }
            else
            {
                if (Time.time - _startTime < _workingTime)
                {
                    var radius = _player.Radius;
                    var pos = _player.BonesConnector.TransformPosition;
                    _trailRenderer.transform.position = pos;
                    _trailRenderer.widthMultiplier = radius * 2;

                    if (Vector2.Distance(_lastTriggerSpawnPoint, pos) > _triggerSpawnDistanceRadiusMult * radius)
                    {
                        SpawnTrigger(pos, radius);
                    }
                }
                else
                {
                    _working = false;
                    ResetTimer();
                }
            }
        }

        private void SpawnTrigger(Vector2 pos, float radius)
        {
            _lastTriggerSpawnPoint = pos;
            var trig = Instantiate(_triggerPrefab, pos, Quaternion.identity, null);
            trig.Init(radius, _triggerDestroyTime, _player.TeamNumber, _player.Health);
        }
    }
}
