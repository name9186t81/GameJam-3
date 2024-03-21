using Abilities;
using Core;
using GameLogic;
using PlayerInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAbilities
{
    
    public class HookAbility : AbilitiesContainer.CooldownAbility, IDirectionalAbility, ISlimeAbility
    {
        private float _maxGrabDist;
        private float _grabSpeed;
        private float _grabTime;
        private LineRenderer _lineRenderer;
        private float _widthOverScale = 0.3f;
        private LayerMask _layerMask;
        private float _returnBackTime = 3f;
        private BoneJointsConnector.SpringJointSettings _springJointSettings;

        private float _currentWidth => _actor.Scale * _widthOverScale;
        private State _currentState = State.Waiting;
        private Vector2 _startGrabDirection;
        private Vector2 _currentPosition;
        private Vector3[] _lrPoses = new Vector3[2];
        private Collider2D _grabbedCollider;
        private Vector2 _grabbedLocalPoint;
        private List<Object> _addedComponents = new List<Object>();
        private Vector3 _smoothDampVel;

        private bool _lineRendererSpawned = false;
        private float _stateChangeTimer = 0;

        public HookAbility(float reloadTime, float maxGrabDist, float grabSpeed, float grabTime, LineRenderer lineRenderer, float widthOverScale, LayerMask layerMask, float returnBackTime, BoneJointsConnector.SpringJointSettings springJointSettings)
        {
            _maxGrabDist = maxGrabDist;
            _grabSpeed = grabSpeed;
            _grabTime = grabTime;
            _lineRenderer = lineRenderer;
            _widthOverScale = widthOverScale;
            _layerMask = layerMask;
            _returnBackTime = returnBackTime;
            _springJointSettings = springJointSettings;
            _reloadTime = reloadTime;

            AbilityType = AbilityType.Instant;
        }

        public SlimeActior Slime { set; private get; }
        public Vector2 Direction { set; private get; }

        public IDirectionalAbility.PrefferedDirectionSource DirectionSource => IDirectionalAbility.PrefferedDirectionSource.CursorDirection;

        private protected override void update(float dt)
        {
            _stateChangeTimer += dt;

            var playerPos = _actor.Position;

            switch (_currentState)
            {
                case State.Waiting:
                    {
                        var time = Mathf.Clamp01(_stateChangeTimer / _returnBackTime);
                        _lineRenderer.gameObject.SetActive(time != 1);
                        _currentPosition = Vector3.SmoothDamp(_currentPosition, playerPos, ref _smoothDampVel, (1 - time) * _returnBackTime);
                    }
                    break;

                case State.Trying:

                    _currentPosition += _startGrabDirection * _grabSpeed * dt * _actor.Scale;

                    if (Vector2.Distance(_currentPosition, playerPos) > _maxGrabDist * _actor.Scale)
                    {
                        ChangeState(State.Waiting);
                    }

                    var overlap = Physics2D.LinecastAll(_currentPosition, playerPos, _layerMask); //_currentWidth

                    for (int i = 0; i < overlap.Length; i++)
                    {
                        if (overlap[i].transform.root != Slime.transform.root)
                        {
                            _grabbedCollider = overlap[i].collider;
                            ChangeState(State.Grabbed);

                            bool bodyAdded = false;
                            if (!_grabbedCollider.TryGetComponent<Rigidbody2D>(out Rigidbody2D body))
                            {
                                body = _grabbedCollider.gameObject.AddComponent<Rigidbody2D>();
                                body.bodyType = RigidbodyType2D.Static;
                                body.mass = 10; //мб не нужно а мб сделать настройку в инспекторе
                                bodyAdded = true;
                            }

                            _grabbedLocalPoint = _grabbedCollider.transform.InverseTransformPoint(overlap[i].point);

                            _addedComponents.Add(Slime.BonesConnector.GrabObject(body, _startGrabDirection, _springJointSettings, _grabbedLocalPoint));
                            if (bodyAdded)
                                _addedComponents.Add(body);

                            break;
                        }
                    }

                    break;

                case State.Grabbed:
                    if (_stateChangeTimer > _grabTime || _grabbedCollider == null)
                    {
                        foreach (var item in _addedComponents)
                        {
                            MonoBehaviour.Destroy(item);
                        }

                        ResetTimer();

                        _addedComponents.Clear();

                        ChangeState(State.Waiting);
                    }
                    else
                    {
                        _currentPosition = _grabbedCollider.transform.TransformPoint(_grabbedLocalPoint);
                    }
                    break;
            }

            if (_lineRendererSpawned)
            {
                _lrPoses[0] = playerPos;
                _lrPoses[1] = _currentPosition;
                _lineRenderer.SetPositions(_lrPoses);
                _lineRenderer.widthMultiplier = _currentWidth;
            }
        }

        private protected override bool canUse()
        {
            return _currentState == State.Waiting && Direction.magnitude > 0.5f;
        }

        public override void Use()
        {
            if(!_lineRendererSpawned)
            {
                _lineRenderer = MonoBehaviour.Instantiate(_lineRenderer, Slime.transform);
                _lineRendererSpawned = true;
            }

            _startGrabDirection = Direction.normalized;
            _currentPosition = Slime.Position;
            ChangeState(State.Trying);
            _lineRenderer.gameObject.SetActive(true);
        }

        private void ChangeState(State newState)
        {
           _currentState = newState;
            _stateChangeTimer = 0;
            //_lineRenderer.gameObject.SetActive(newState != State.Waiting);
        }

        private enum State
        {
           Waiting,
           Trying,
           Grabbed
        }
    }
}