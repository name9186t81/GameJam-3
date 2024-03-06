using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Movement;

namespace GameLogic
{
    public class PlayerActor : MonoBehaviour, IActor, IMovable
    {
        [SerializeField] private BoneJointsConnector _body;

        [SerializeField] private Camera _camera;
        [SerializeField] private AnimationCurve _cameraSize;
        [SerializeField] private float _cameraSizeMult = 5;
        [SerializeField] private float _cameraSizeSmoothTime = 0.1f;

        [SerializeField] private float _moveForce = 5000;
        [SerializeField] private float _moveForcePerCombo = 100;
        [SerializeField] private float _moveForceComboLimit = 3000;
        [SerializeField] private AnimationCurve _scoreMultPerCombo;
        [SerializeField] private float _slowdownFactor;
        [SerializeField] private float _dashForceMult = 80;
        [SerializeField] private AnimationCurve _dashForce;
        [SerializeField] private float _startBodySize;
        [SerializeField] private float _testAreaPerCollision = 0.1f;

        private float _cameraSizeVelocity = 0f;

        public Vector2 position => _body.position;
        public Vector2 velocity { get { return _body.velocity; } set { _body.velocity = value; } }
        public float rotation { get { return 0; } set { } }

        public float CurrentScore => _body.CurrentScale * _body.CurrentScale;
        public IController Controller { get; private set; }

        public event Action<ControllerAction> OnAction;
        public event Action<float> OnAddScore;

        private void Awake()
        {
            OnAction += OnControllerAction;
            _body.OnCollisionEnter += OnBodyCollisionEnter;
        }

        private void Start()
        {
            _body.Size = _startBodySize;
        }

        private void FixedUpdate()
        {
            var direction = (this as IActor).DesiredMoveDirection;

            _body.AddForceToAll(-_body.velocity * _slowdownFactor * Time.fixedDeltaTime);
            _body.AddForce(direction * (_moveForce + MathF.Min(_moveForceComboLimit, _moveForcePerCombo * ComboUI.ComboCount)) * Time.fixedDeltaTime);
        }

        private void Update()
        {
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, _body.CurrentScale * _cameraSize.Evaluate(_body.Size) * _cameraSizeMult, ref _cameraSizeVelocity, _cameraSizeSmoothTime);
        }

        private void OnBodyCollisionEnter(Collision2D collision)
        {
            AddScore(_testAreaPerCollision);
        }

        public void AddScore(float score)
        {
            score = score * _scoreMultPerCombo.Evaluate(ComboUI.ComboCount);
            _body.AddArea(score);
            OnAddScore?.Invoke(score);
        }

        private void OnControllerAction(ControllerAction action)
        {
            switch (action)
            {
                case ControllerAction.Dash:
                    _body.AddForceToAll(Controller.DesiredMoveDirection * _dashForce.Evaluate(_body.Size) * _dashForceMult, ForceMode2D.Impulse);
                    break;
            }
        }

        public bool TryChangeController(in IController controller)
        {
            if(Controller != null)
                Controller.OnAction -= Act;

            this.Controller = controller;
            controller.OnAction += Act;
            return true;
        }

        public void Act(ControllerAction obj)
        {
            OnAction?.Invoke(obj);
        }
    }
}