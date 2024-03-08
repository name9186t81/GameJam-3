using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Movement;
using Health;
using PlayerAbilities;

namespace GameLogic
{
    [RequireComponent(typeof(SlimeHealth))]
    public class PlayerActor : MonoBehaviour, IActor, IMovable, ITeamProvider //ITeamProvider пытается перебрасывать все с IHealth и нужен он тут потому что без скрипта одновременно с актором и провайдером ии еды не будет работать
    {
        public SlimeHealth Health { get; private set; }
        [SerializeField] private BoneJointsConnector _body;
        [SerializeField] private SpriteRenderer _bodySprite;
        [SerializeField] private int _flyingSpriteSortingLayer;

        [SerializeField] private Camera _camera;
        [SerializeField] private AnimationCurve _cameraSize;
        [SerializeField] private float _cameraSizeMult = 5;
        [SerializeField] private float _cameraSizeSmoothTime = 0.1f;

        [SerializeField] private float _moveForce = 5000;
        [SerializeField] private float _moveForcePerCombo = 100;
        [SerializeField] private float _moveForceComboLimit = 3000;
        [SerializeField] private float _dashForceMult = 80;
        [SerializeField] private AnimationCurve _dashForce;
        [SerializeField] private float _startBodySize;
        [SerializeField] private float _testAreaPerCollision = 0.1f;
        [SerializeField] private float _slimeMergeForcePerScore = 1f;

        private float _cameraSizeVelocity = 0f;
        private float _startFlyingSize;
        private int _defaultSpriteSortingLayer;

        public Vector2 Position { get => _body.Position; set { _body.Position = value; } }
        Vector2 IActor.Position { get => _body.TransformPosition; } //da
        public Vector2 Velocity { get => _body.Velocity; set { _body.Velocity = value; } }
        public float Rotation { get => 0; set { } }
        public float Radius => Health.Radius;
        public float Scale => _body.CurrentScale;
        public BoneJointsConnector BonesConnector => _body;
        public float CurrentScore => _body.CurrentScale * _body.CurrentScale;

        public IController Controller { get; private set; }
        public IActor Actor { get => this; set => throw new NotImplementedException(); }

        public int TeamNumber => Health.TeamNumber;

        public event Action<ControllerAction> OnAction;
        public event Action OnInit;
        public event Action<int, int> OnTeamNumberChange;

        private void Awake()
        {
            Health = GetComponent<SlimeHealth>();
            OnAction += OnControllerAction;
            _defaultSpriteSortingLayer = _bodySprite.sortingOrder;
            OnInit?.Invoke();
        }

        private void Start()
        {
            _body.Size = _startBodySize;
        }

        private void FixedUpdate()
        {
            var direction = (this as IActor).DesiredMoveDirection;

            _body.AddForce(direction * (_moveForce + MathF.Min(_moveForceComboLimit, _moveForcePerCombo * ComboUI.ComboCount)) * Time.fixedDeltaTime);
        }

        private void Update()
        {
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, _body.CurrentScale * _cameraSize.Evaluate(_body.Size) * _cameraSizeMult, ref _cameraSizeVelocity, _cameraSizeSmoothTime);
        }

        public void OnSlimeCollision(SlimeHealth slime, BoneJointsConnector body, Vector2 collisionPoint)
        {
            if(PlayerPetSlimesAbility.TryReturnSlime(body, _body))
            {
                _body.AddForceToNearestBone(collisionPoint, (body.Position - Position).normalized * _slimeMergeForcePerScore * CurrentScore);
                Destroy(slime.gameObject); //прощай слаймик!!
            }
        }

        public void SetFlyingState(bool flying)
        {
            if (flying)
                _startFlyingSize = _body.Size;
            else
                _body.Size = _startFlyingSize;

            _body.SetIgnoreWorldCollision(flying);
            _bodySprite.sortingOrder = flying ? _flyingSpriteSortingLayer : _defaultSpriteSortingLayer;
        }

        public void SetFlyingSizeMult(float mult)
        {
            _body.Size = _startFlyingSize * mult;
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

        public bool TryChangeTeamNumber(int newTeamNumber)
        {
            OnTeamNumberChange?.Invoke(TeamNumber, newTeamNumber);
            return Health.TryChangeTeamNumber(newTeamNumber);
        }
    }
}