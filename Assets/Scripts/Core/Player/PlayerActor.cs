using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace GameLogic
{
    public class PlayerActor : MonoBehaviour, IActor
    {
        [SerializeField] private BoneJointsConnector _body;

        [SerializeField] private float _moveForce;
        [SerializeField] private float _slowdownFactor;
        [SerializeField] private float _dashForceMult = 80;
        [SerializeField] private AnimationCurve _dashForce;

        public Vector2 Position => _body.position;

        public IController Controller { get; private set; }

        public event Action<ControllerAction> OnAction;

        private void Awake()
        {
            OnAction += OnControllerAction;
        }

        private void FixedUpdate()
        {
            var direction = (this as IActor).DesiredMoveDirection;

            _body.AddForceToAll(-_body.velocity * _slowdownFactor * Time.fixedDeltaTime);
            _body.AddForce(direction * _moveForce * Time.fixedDeltaTime);
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

        private void Act(ControllerAction obj)
        {
            OnAction?.Invoke(obj);
        }
    }
}