using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAbilities
{
    public class PlayerJumpAbility : AbilitiesContainer.Ability, TimeScaleController.ITimeScaleMultiplyer
    {
        [SerializeField] private float _placeSelectTime = 0.5f;
        [SerializeField] private float _timePauseSmoothTime = 0.1f;
        [SerializeField] private JumpPointSelector _jumpPointSelector;
        [SerializeField] private float _overlapRadiusMult = 0.6f;

        [SerializeField] private float _jumpTime = 0.7f;
        [SerializeField] private AnimationCurve _jumpProgressCurve;
        [SerializeField] private AnimationCurve _slimeSizeCurve;

        State _currentState;
        FloatSmoothDamp _timeSmooth;
        float _lastStateChangeTime = 0;
        Vector2 _startPosition;
        Vector2 _targetPosition;

        public float TimeScale { get; private set; } = 1;

        private bool _usePointerEvents => _usingMobileInput;

        void Start()
        {
            _timeSmooth = new FloatSmoothDamp(_timePauseSmoothTime, 1);
            TimeScaleController.Add(this);
        }

        void OnDestroy()
        {
            TimeScaleController.Remove(this);
        }

        void FixedUpdate()
        {
            switch (_currentState)
            {
                case State.Jumping:
                    var timeSinceStart = Time.unscaledTime - _lastStateChangeTime;

                    if (timeSinceStart < _jumpTime)
                    {
                        //_player.Position = Vector3.Lerp(_startPosition, _targetPosition, _jumpProgressCurve.Evaluate(timeSinceStart / _jumpTime));
                    }
                    break;
            }
        }

        private protected override void update()
        {
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
                            _player.SetFlyingState(true);
                            _startPosition = _player.Position;
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
                        _player.SetFlyingState(false);
                        ChangeState(State.Waiting);
                        ResetTimer();
                    }
                    else
                    {
                        _player.SetFlyingSizeMult(_slimeSizeCurve.Evaluate(timeSinceStart / _jumpTime));
                        _player.Position = Vector3.Lerp(_startPosition, _targetPosition, _jumpProgressCurve.Evaluate(timeSinceStart / _jumpTime));
                    }
                    break;
            }

            bool timeStopState = _currentState == State.Selecting && Time.unscaledTime - _lastStateChangeTime < _placeSelectTime;
            TimeScale = _timeSmooth.Update(timeStopState ? 0 : 1);
        }

        private void DoDamageInRadius()
        {
            var damageRadius = _player.Radius;
            var position = _player.Position;

            var colliders = Physics2D.OverlapCircleAll(position, damageRadius);

            foreach(var col in colliders)
            {

            }

            //Debug.LogError("��� �������� ���� ���� ��������!!");
            //���� ��� ����
        }

        private void ChangeState(State newState)
        {
            _currentState = newState;
            _lastStateChangeTime = Time.unscaledTime;
            _jumpPointSelector.gameObject.SetActive(newState == State.Selecting);
            _jumpPointSelector.SetRadius(_player.Radius * _overlapRadiusMult);
            _jumpPointSelector.PositionProvider = _usePointerEvents ? EventDataPositionProvider : MousePositionProvider;
        }

        private Vector2 EventDataPositionProvider()
        {
            if(_lastPointerEventData == null)
                return MousePositionProvider();

            return Camera.main.ScreenToWorldPoint(_lastPointerEventData.position);
        }

        private Vector2 MousePositionProvider()
        {
            return _cursorWorldPos;
        }

        private enum State
        {
            Waiting,
            Selecting,
            Jumping
        }
    }
}