using Abilities;
using AI;
using GameLogic;
using Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAbilities
{
    public class PetSlimesAbility : AbilitiesContainer.CooldownAbility, ISlimeAbility
    {
        private AIController _petSlimePrefab;
        private int _slimesCount;
        private float _radiusSpawnOffset;
        private float _pullScaleFromPlayer;
        private float _getScaleFromPlayer;
        private float _slimeCanBeReturnedTime;

        public SlimeActior Slime { set; private get; }

        public PetSlimesAbility(float reloadTime, AIController petSlimePrefab, int slimesCount, float radiusSpawnOffset, float pullScaleFromPlayer, float getScaleFromPlayer, float slimeCanBeReturnedTime)
        {
            _petSlimePrefab = petSlimePrefab;
            _slimesCount = slimesCount;
            _radiusSpawnOffset = radiusSpawnOffset;
            _pullScaleFromPlayer = pullScaleFromPlayer;
            _getScaleFromPlayer = getScaleFromPlayer;
            _slimeCanBeReturnedTime = slimeCanBeReturnedTime;

            AbilityType = AbilityType.Instant;
            _reloadTime = reloadTime;
        }

        public override void Use()
        {
            var radius = _actor.Radius;

            var _ScalePulledFromPlayer = Slime.BonesConnector.Scale * _pullScaleFromPlayer;

            var ScalePerSlime = (Slime.BonesConnector.Scale * _getScaleFromPlayer + _ScalePulledFromPlayer) / _slimesCount;

            Slime.BonesConnector.Scale -= _ScalePulledFromPlayer;

            for (int i = 0; i < _slimesCount; i++)
            {
                var angle = i * (Mathf.PI * 2 / _slimesCount);
                var vector = Vector2Extensions.VectorFromAngle(angle);

                var slime = MonoBehaviour.Instantiate(_petSlimePrefab, _actor.Position, Quaternion.identity, null);
                slime.gameObject.SetActive(true);
                //slime.GetComponent<IMovable>().Position = pos;

                var body = slime.GetComponent<BoneJointsConnector>();
                var health = slime.GetComponent<SlimeHealth>();
                var actor = slime.GetComponent<SlimeActior>();

                body.Scale = ScalePerSlime;
                slime.Owner = _actor;

                var pos = _actor.Position + vector * ((_actor.Radius + health.Radius) * (1 + _radiusSpawnOffset));

                body.transform.position = pos;

                actor.PetSlimeReturnData = new SlimeReturnData(_ScalePulledFromPlayer / _slimesCount, body.Scale, Time.time + _slimeCanBeReturnedTime);
            }

            ResetTimer();
        }

        private protected override void update(float dt)
        {
            
        }

        public class SlimeReturnData
        {
            private float _ScaleToReturn;
            private float _startSlimeScale;
            private float _slimeCanBeReturnedTime;
            bool returned = false;

            public bool TryReturn(BoneJointsConnector bodyToReturn, BoneJointsConnector bodyToReturnFrom)
            {
                if (returned)
                    return false;

                if(Time.time > _slimeCanBeReturnedTime)
                {
                    var slimeLooted = Mathf.Max(0, bodyToReturnFrom.Scale - _startSlimeScale);
                    bodyToReturn.Scale += _ScaleToReturn + slimeLooted;
                    returned = true;
                    return true;
                }

                return false;
            }

            public SlimeReturnData(float ScaleToReturn, float startSlimeScale, float slimeCanBeReturnedTime)
            {
                _ScaleToReturn = ScaleToReturn;
                _startSlimeScale = startSlimeScale;
                _slimeCanBeReturnedTime = slimeCanBeReturnedTime;
            }
        }
    }
}