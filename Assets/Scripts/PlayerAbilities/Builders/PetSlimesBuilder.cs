using Abilities;
using AI;
using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAbilities
{
    [CreateAssetMenu(fileName = "Pet slimes ability", menuName = "GameJam/Pet slimes")]
    public class PetSlimesBuilder : AbilityBuilder
    {
        [SerializeField] private protected float _reloadTime = 30f;
        [SerializeField] private AIController _petSlimePrefab;
        [SerializeField] private int _slimesCount;
        [SerializeField] private float _radiusSpawnOffset = 0.5f;
        [SerializeField][Range(0, 1)] private float _pullScaleFromPlayer = 0.4f;
        [SerializeField][Range(0, 1)] private float _getScaleFromPlayer = 0.2f;
        [SerializeField] private float _slimeCanBeReturnedTime = 15;

        public override IAbility Build(IActor owner)
        {
            var ability = new PetSlimesAbility(_reloadTime, _petSlimePrefab, _slimesCount, _radiusSpawnOffset, _pullScaleFromPlayer, _getScaleFromPlayer, _slimeCanBeReturnedTime);
            ability.Init(owner);
            return ability;
        }
    }
}
