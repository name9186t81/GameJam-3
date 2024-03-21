using Abilities;
using Core;
using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlayerAbilities
{
    [CreateAssetMenu(fileName = "Hook ability", menuName = "GameJam/Hook")]
    public class HookBuilder : AbilityBuilder
    {
        [SerializeField] private protected float _reloadTime = 2.5f;
        [SerializeField] private float _maxGrabDist;
        [SerializeField] private float _grabSpeed;
        [SerializeField] private float _grabTime;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _widthOverScale = 0.3f;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private float _returnBackTime = 3f;
        [SerializeField] private BoneJointsConnector.SpringJointSettings _springJointSettings;

        public override IAbility Build(IActor owner)
        {
            var ability = new HookAbility(_reloadTime, _maxGrabDist, _grabSpeed, _grabTime, _lineRenderer, _widthOverScale, _layerMask, _returnBackTime, _springJointSettings);
            ability.Init(owner);
            return ability;
        }
    }
}