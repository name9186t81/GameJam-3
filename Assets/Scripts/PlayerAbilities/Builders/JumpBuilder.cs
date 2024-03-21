using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using Core;

namespace PlayerAbilities
{
    [CreateAssetMenu(fileName = "Jump ability", menuName = "GameJam/Jump")]
    public class JumpBuilder : AbilityBuilder
    {
        [SerializeField] private protected float _reloadTime = 10f;
        [SerializeField] private float _jumpTime = 0.7f;
        [SerializeField] private AnimationCurve _jumpProgressCurve;
        [SerializeField] private AnimationCurve _slimeSizeCurve;

        public override IAbility Build(IActor owner)
        {
            var ability = new JumpAbility(_jumpTime, _jumpProgressCurve, _slimeSizeCurve, _reloadTime);
            ability.Init(owner);
            return ability;
        }
    }
}