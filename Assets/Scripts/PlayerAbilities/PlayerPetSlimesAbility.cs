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
        [SerializeField] [Range(0,1)] private float _pullSizeFromPlayer = 0.4f;
        [SerializeField] [Range(0,1)] private float _getSizeFromPlayer = 0.2f;
        [SerializeField] private float _slimeCanBeReturnedTime = 15;

        private static Dictionary<BoneJointsConnector, SlimeReturnData> _spawnedSlimes = new Dictionary<BoneJointsConnector, SlimeReturnData>();
        private class SlimeReturnData
        {
            private float _sizeToReturn;
            private float _startSlimeSize;
            private float _slimeCanBeReturnedTime;

            public bool Return(BoneJointsConnector bodyToReturn, BoneJointsConnector bodyToReturnFrom)
            {
                if(Time.time > _slimeCanBeReturnedTime)
                {
                    bodyToReturn.Size += _sizeToReturn + Mathf.Max(0, bodyToReturnFrom.Size - _startSlimeSize);
                    return true;
                }

                return false;
            }

            public SlimeReturnData(float sizeToReturn, float startSlimeSize, float slimeCanBeReturnedTime)
            {
                _sizeToReturn = sizeToReturn;
                _startSlimeSize = startSlimeSize;
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
            if(TryUse())
            {
                var radius = _player.Radius;

                var _sizePulledFromPlayer = _player.BonesConnector.Size * _pullSizeFromPlayer;

                var sizePerSlime = (_player.BonesConnector.Size * _getSizeFromPlayer + _sizePulledFromPlayer) / _slimesCount;

                _player.BonesConnector.Size -= _sizePulledFromPlayer;

                for (int i = 0; i < _slimesCount; i++)
                {
                    var angle = i * (Mathf.PI * 2 / _slimesCount);
                    var vector = Vector2Extensions.VectorFromAngle(angle);

                    var pos = _player.Position + vector * (radius * (1 + _radiusSpawnOffset));

                    var slime = Instantiate(_petSlimePrefab, pos, Quaternion.identity, null);
                    slime.gameObject.SetActive(true);
                    var body = slime.GetComponent<BoneJointsConnector>();
                    body.Size = sizePerSlime;
                    //slime.GetComponent<IMovable>().Position = pos;
                    slime.Owner = _player;

                    _spawnedSlimes.Add(body, new SlimeReturnData(_sizePulledFromPlayer / _slimesCount, sizePerSlime, Time.time + _slimeCanBeReturnedTime));
                }
            }
        }

        //public static bool TryColl
    }
}