using Abilities;
using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAbilities
{
    public class JumpAbility : AbilitiesContainer.CooldownAbility, TimeScaleController.ITimeScaleMultiplyer, IPositionalAbility, ISlimeAbility
    {
        //[SerializeField] private float _placeSelectTime = 0.5f;
        //[SerializeField] private float _timePauseSmoothTime = 0.1f;

        [SerializeField] private float _jumpTime = 0.7f;
        [SerializeField] private AnimationCurve _jumpProgressCurve;
        [SerializeField] private AnimationCurve _slimeSizeCurve;

        //FloatSmoothDamp _timeSmooth;
        Vector2 _startPosition;
        Vector2 _targetPosition;

        public float TimeScale { get; private set; } = 1;

        public Vector2 WorldPosition { set; private get; }
        public SlimeActior Slime { set; private get; }

        private float _useTimer;
        private float _useProgressUnclamped => _useTimer / _jumpTime;

        bool _jumping;

        public JumpAbility(float jumpTime, AnimationCurve jumpProgressCurve, AnimationCurve slimeSizeCurve, float reloadTime)
        {
            _jumpTime = jumpTime;
            _jumpProgressCurve = jumpProgressCurve;
            _slimeSizeCurve = slimeSizeCurve;
            _reloadTime = reloadTime;

            AbilityType = AbilityType.Instant;
        }

        private protected override void init()
        {
            //_timeSmooth = new FloatSmoothDamp(_timePauseSmoothTime, 1);
            //TimeScaleController.Add(this);
        }

        private protected override void update(float dt)
        {
            _useTimer += dt;

            if(_jumping && _useProgressUnclamped >= 1)
            {
                EndJumping();
            }

            if(_jumping)
            {
                ResetTimer();
                Slime.SetFlyingSizeMult(_slimeSizeCurve.Evaluate(_useProgressUnclamped));
                Slime.Position = Vector3.Lerp(_startPosition, _targetPosition, _jumpProgressCurve.Evaluate(_useProgressUnclamped));
            }

            //bool timeStopState = _currentState == State.Selecting && Time.unscaledTime - _lastStateChangeTime < _placeSelectTime;
            //TimeScale = _timeSmooth.Update(timeStopState ? 0 : 1);

            {
                /*
                switch(_currentState)
                {
                    case State.Waiting:
                        if (CanUse(false))
                        {
                            ChangeState(State.Selecting);
                        }
                        break;
                    case State.Selecting:
                        _jumpPointSelector.Update();
                        if((_usePointerEvents ? (_lastPointerEventData != null && _lastPointerEventData.used) : Input.GetMouseButtonDown(0)))
                        {
                            if (_jumpPointSelector.CanJump)
                            {
                                ChangeState(State.Jumping);
                                //_actor.SetFlyingState(true);
                                _startPosition = _actor.Position;
                                _targetPosition = _jumpPointSelector.transform.position;
                            }
                            else
                            {
                                ChangeState(State.Waiting);
                            }
                        }
                        break;
                    case State.Jumping:
                        var timeSinceStart = Time.unscaledTime - _lastStateChangeTime;

                        if(timeSinceStart >= _jumpTime)
                        {
                            DoDamageInRadius();
                            //_actor.SetFlyingState(false);
                            ChangeState(State.Waiting);
                            ResetTimer();
                        }
                        else
                        {
                            //_actor.SetFlyingSizeMult(_slimeSizeCurve.Evaluate(timeSinceStart / _jumpTime));
                            //_actor.Position = Vector3.Lerp(_startPosition, _targetPosition, _jumpProgressCurve.Evaluate(timeSinceStart / _jumpTime));
                        }
                        break;
                }
                */
            }
        }

        void StartJumping()
        {
            _jumping = true;
            Slime.SetFlyingState(true);
            _startPosition = _actor.Position;
            _targetPosition = WorldPosition;
        }

        void EndJumping()
        {
            _jumping = false;
            DoDamageInRadius();
            Slime.SetFlyingState(false);
        }

        private void DoDamageInRadius()
        {
            var damageRadius = _actor.Radius;
            var position = _actor.Position;

            var colliders = Physics2D.OverlapCircleAll(position, damageRadius);

            foreach(var col in colliders)
            {

            }

            //Debug.LogError("тут наверное урон надо наносить!!");
            //ноуп мне лень
        }

        public override void Use()
        {
            _useTimer = 0;
            StartJumping();
        }
    }
}