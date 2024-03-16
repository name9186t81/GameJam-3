using AI;
using GameLogic;
using Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAbilities
{
    public class PlayerPetSlimesAbility : AbilitiesContainer.Ability
    {
        [SerializeField] private AIController _petSlimePrefab;
        [SerializeField] private int _slimesCount;
        [SerializeField] private float _radiusSpawnOffset = 0.5f;
        [SerializeField] [Range(0,1)] private float _pullScaleFromPlayer = 0.4f;
        [SerializeField] [Range(0,1)] private float _getScaleFromPlayer = 0.2f;
        [SerializeField] private float _slimeCanBeReturnedTime = 15;

        private static Dictionary<BoneJointsConnector, SlimeReturnData> _spawnedSlimes = new Dictionary<BoneJointsConnector, SlimeReturnData>();
        private class SlimeReturnData
        {
            private float _ScaleToReturn;
            private float _startSlimeScale;
            private float _slimeCanBeReturnedTime;

            public bool Return(BoneJointsConnector bodyToReturn, BoneJointsConnector bodyToReturnFrom)
            {
                if(Time.time > _slimeCanBeReturnedTime)
                {
                    var slimeLooted = Mathf.Max(0, bodyToReturnFrom.Scale - _startSlimeScale);
                    bodyToReturn.Scale += _ScaleToReturn + slimeLooted;
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

        public static bool TryReturnSlime(BoneJointsConnector otherSlime, BoneJointsConnector player)
        {
            if(_spawnedSlimes.TryGetValue(otherSlime, out SlimeReturnData slimeReturnData) && slimeReturnData.Return(player, otherSlime))
            {
                _spawnedSlimes.Remove(otherSlime);
                return true;
            }

            return false;
        }

        private protected override void update()
        {
            if(CanUse())
            {
                var radius = _player.Radius;

                var _ScalePulledFromPlayer = _player.BonesConnector.Scale * _pullScaleFromPlayer;

                var ScalePerSlime = (_player.BonesConnector.Scale * _getScaleFromPlayer + _ScalePulledFromPlayer) / _slimesCount;

                _player.BonesConnector.Scale -= _ScalePulledFromPlayer;

                for (int i = 0; i < _slimesCount; i++)
                {
                    var angle = i * (Mathf.PI * 2 / _slimesCount);
                    var vector = Vector2Extensions.VectorFromAngle(angle);

                    var slime = Instantiate(_petSlimePrefab, _player.Position, Quaternion.identity, null);
                    slime.gameObject.SetActive(true);
                    var body = slime.GetComponent<BoneJointsConnector>();
                    body.Scale = ScalePerSlime;
                    //slime.GetComponent<IMovable>().Position = pos;
                    slime.Owner = _player;
                    var health = slime.GetComponent<SlimeHealth>();

                    var pos = _player.Position + vector * ((_player.Radius + health.Radius) * (1 + _radiusSpawnOffset));

                    body.transform.position = pos;

                    _spawnedSlimes.Add(body, new SlimeReturnData(_ScalePulledFromPlayer / _slimesCount, body.Scale, Time.time + _slimeCanBeReturnedTime));
                }
            }
        }

        //public static bool TryColl
    }
}