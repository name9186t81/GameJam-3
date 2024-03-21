using Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAbilities
{
    public class TrailAbility : AbilitiesContainer.CooldownAbility, ISlimeAbility
    {
        private float _workingTime;
        private float _triggerDestroyTime;
        private TrailRenderer _trailRenderer;
        private float _triggerSpawnDistanceRadiusMult = 1;
        private DamagingTrigger _triggerPrefab;

        private float _timer;
        private bool _working;
        private Vector2 _lastTriggerSpawnPoint;
        private bool _trailRendererSpawned = false;

        public TrailAbility(float reloadTime, float workingTime, float triggerDestroyTime, TrailRenderer trailRenderer, float triggerSpawnDistanceRadiusMult, DamagingTrigger triggerPrefab)
        {
            _workingTime = workingTime;
            _triggerDestroyTime = triggerDestroyTime;
            _trailRenderer = trailRenderer;
            _triggerSpawnDistanceRadiusMult = triggerSpawnDistanceRadiusMult;
            _triggerPrefab = triggerPrefab;

            AbilityType = AbilityType.Instant;
            _reloadTime = reloadTime;
        }

        public SlimeActior Slime { set; private get; }

        public override void Use()
        {
            if(!_trailRendererSpawned)
            {
                _trailRenderer = MonoBehaviour.Instantiate(_trailRenderer, Slime.transform);
                _trailRenderer.time = _triggerDestroyTime;
                _trailRenderer.transform.SetParent(null);
                _trailRendererSpawned = true;
            }

            _working = true;
            _trailRenderer.transform.position = Slime.TransformPosition;
            _trailRenderer.Clear();
            _lastTriggerSpawnPoint = Vector2.one * float.MaxValue;
            _timer = 0;
        }

        private protected override bool canUse()
        {
            return !_working;
        }

        private protected override void update(float dt)
        {
            _timer += dt;

            if(_working)
            {
                ResetTimer();

                if (_timer < _workingTime)
                {
                    var radius = _actor.Radius;
                    var pos = Slime.TransformPosition;
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
                }
            }
        }

        private void SpawnTrigger(Vector2 pos, float radius)
        {
            _lastTriggerSpawnPoint = pos;
            var trig = MonoBehaviour.Instantiate(_triggerPrefab, pos, Quaternion.identity, null);
            trig.Init(radius, _triggerDestroyTime, Slime.TeamNumber, Slime.Health);
        }
    }
}
