using GameLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAbilities
{
    public class PlayerGrabHandAbility : AbilitiesContainer.Ability
    {
        [SerializeField] private float _maxGrabDist;
        [SerializeField] private float _grabSpeed;
        [SerializeField] private float _grabTime;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _widthOverScale = 0.3f;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private float _returnBackTime = 3f;
        [SerializeField] private BoneJointsConnector.SpringJointSettings _springJointSettings;

        private float _currentWidth => _player.Scale * _widthOverScale;
        private State _currentState = State.Waiting;
        private Vector2 _startGrabDirection;
        private Vector2 _currentPosition;
        private float _lastStateChangeTime = 0;
        private Vector3[] _lrPoses = new Vector3[2];
        private Collider2D _grabbedCollider;
        private Vector2 _grabbedLocalPoint;
        private List<Object> _addedComponents = new List<Object>();

        private protected override void update()
        {
            var playerPos = _player.Position;

            switch (_currentState)
            {
                case State.Waiting:
                    if (TryUse(false))
                    {
                        _startGrabDirection = (_cursorWorldPos - playerPos).normalized;
                        _currentPosition = playerPos;
                        ChangeState(State.Trying);
                    }
                    else
                    {
                        _currentPosition = Vector3.Lerp(_currentPosition, playerPos, Mathf.Clamp01((Time.time - _lastStateChangeTime) / _returnBackTime));
                    }
                    break;

                case State.Trying:

                    _currentPosition += _startGrabDirection * _grabSpeed * Time.deltaTime * _player.Scale;

                    if (Vector2.Distance(_currentPosition, playerPos) > _maxGrabDist * _player.Scale)
                    {
                        ChangeState(State.Waiting);
                    }

                    var overlap = Physics2D.LinecastAll(_currentPosition, playerPos, _layerMask); //_currentWidth

                    for (int i = 0; i < overlap.Length; i++)
                    {
                        if (overlap[i].transform.root != _player.transform.root)
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

                            _addedComponents.Add(_player.BonesConnector.GrabObject(body, _startGrabDirection, _springJointSettings, _grabbedLocalPoint));
                            if (bodyAdded)
                                _addedComponents.Add(body);

                            break;
                        }
                    }

                    break;

                case State.Grabbed:
                    if (Time.time - _lastStateChangeTime > _grabTime || _grabbedCollider == null)
                    {
                        foreach (var item in _addedComponents)
                        {
                            Destroy(item);
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

            _lrPoses[0] = playerPos;
            _lrPoses[1] = _currentPosition;
            _lineRenderer.SetPositions(_lrPoses);
            _lineRenderer.widthMultiplier = _currentWidth;
        }

        private void ChangeState(State newState)
        {
            _currentState = newState;
            _lastStateChangeTime = Time.time;
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