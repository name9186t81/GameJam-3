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

        private static float _sizePulledFromPlayer;

        private protected override void update()
        {
            if(TryUse())
            {
                var radius = _player.Radius;

                _sizePulledFromPlayer = _player.BonesConnector.Size * _pullSizeFromPlayer;

                var sizePerSlime = (_player.BonesConnector.Size * _getSizeFromPlayer + _sizePulledFromPlayer) / _slimesCount;

                _player.BonesConnector.Size -= _sizePulledFromPlayer;

                for (int i = 0; i < _slimesCount; i++)
                {
                    var angle = i * (Mathf.PI * 2 / _slimesCount);
                    var vector = Vector2Extensions.VectorFromAngle(angle);

                    var pos = _player.Position + vector * (radius * (1 + _radiusSpawnOffset));

                    var slime = Instantiate(_petSlimePrefab, pos, Quaternion.identity, null);
                    slime.gameObject.SetActive(true);
                    slime.GetComponent<BoneJointsConnector>().Size = sizePerSlime;
                    //slime.GetComponent<IMovable>().Position = pos;
                    slime.Owner = _player;
                }
            }
        }

        //public static bool TryColl
    }
}