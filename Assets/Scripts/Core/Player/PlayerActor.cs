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
    public class PlayerActor : MonoBehaviour, IActor, IMovable, ITeamProvider, //ITeamProvider пытается перебрасывать все с IHealth и нужен он тут потому что без скрипта одновременно с актором и провайдером ии еды не будет работать
    IProvider<IHealth> //todo: релизовать другие интерфейсы по типу этого
        {
        public SlimeHealth Health { get; private set; }
        [SerializeField] private BoneJointsConnector _body;
        [SerializeField] private SpriteRenderer _bodySprite;
        [SerializeField] private int _flyingSpriteSortingLayer;

        [SerializeField] private float _moveForce = 5000;
        [SerializeField] private float _moveForcePerCombo = 100;
        [SerializeField] private float _moveForceComboLimit = 3000;
        [SerializeField] private float _dashForceMult = 80;
        [SerializeField] private AnimationCurve _dashForce;

        public Vector2 Position { get => _body.Position; set { _body.Position = value; } }
        public Vector2 Velocity { get => _body.Velocity; set { _body.Velocity = value; } }
        public float Rotation { get => 0; set { } }
        public float Radius => Health.Radius;
        public float Scale => _body.Scale;
        float IActor.Scale => Radius;
        public BoneJointsConnector BonesConnector => _body;
        public float CurrentScore => _body.Scale * _body.Scale;

        public IController Controller { get; private set; }
        public IActor Actor { get => this; set => throw new NotImplementedException(); }

        public int TeamNumber => Health.TeamNumber;

		public string Name => "Player";

		public IHealth Value => Health;

		public event Action<ControllerAction> OnAction;
        public event Action OnInit;
        public event Action<int, int> OnTeamNumberChange;

        private Vector2 _lastActualDesiredMoveDirection;

        private void Awake()
        {
            Health = GetComponent<SlimeHealth>();
            OnAction += OnControllerAction;
            OnInit?.Invoke();
        }

        private void FixedUpdate()
        {
            var direction = (this as IActor).DesiredMoveDirection;

            if(direction.magnitude > 0.5f)
                _lastActualDesiredMoveDirection = direction;

            _body.AddForce(direction * (_moveForce + MathF.Min(_moveForceComboLimit, _moveForcePerCombo * ComboUI.ComboCount)) * Time.fixedDeltaTime);
        }

        private void OnControllerAction(ControllerAction action)
        {
            switch (action)
            {
                case ControllerAction.Dash:
                    _body.AddForceToAll(_lastActualDesiredMoveDirection.normalized * _dashForce.Evaluate(_body.Size) * _dashForceMult, ForceMode2D.Impulse);
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