using Abilities;
using Core;
using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlayerAbilities
{
    [CreateAssetMenu(fileName = "Trail ability", menuName = "GameJam/Trail ability")]
    public class TrailBuilder : AbilityBuilder
    {
        [SerializeField] private protected float _reloadTime = 15f;
        [SerializeField] private float _workingTime;
        [SerializeField] private float _triggerDestroyTime;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField][Range(0, 2)] private float _triggerSpawnDistanceRadiusMult = 1;
        [SerializeField] private DamagingTrigger _triggerPrefab;

        public override IAbility Build(IActor owner)
        {
            var ability = new TrailAbility(_reloadTime, _workingTime, _triggerDestroyTime, _trailRenderer, _triggerSpawnDistanceRadiusMult, _triggerPrefab);
            ability.Init(owner);
            return ability;
        }
    }
}
