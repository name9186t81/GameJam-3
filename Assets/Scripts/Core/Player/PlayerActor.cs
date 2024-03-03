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
        [SerializeField][Range(0, 1)] private float _slowdownFactor;
        [SerializeField] private float _boostForce;

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

            var fixedsPerSecond = 1 / Time.fixedDeltaTime;

            _body.AddForce(-_body.velocity * _slowdownFactor * fixedsPerSecond + direction * _moveForce * Time.fixedDeltaTime);
        }

        private void OnControllerAction(ControllerAction action)
        {
            switch (action)
            {
                case ControllerAction.Dash:
                    _body.AddForce(Controller.DesiredMoveDirection * _boostForce, ForceMode2D.Impulse);
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