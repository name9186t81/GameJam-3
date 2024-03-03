using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class PlayerActor : MonoBehaviour, IActor
{
    [SerializeField] private BoneJointsConnector body;

    [SerializeField] private float MoveForce;
    [SerializeField] [Range(0,1)] private float SlowDownCoefficient;
    [SerializeField] private float BoostForce;

    public Vector2 Position => body.position;

    public IController Controller => controller;
    private IController controller = new PlayerInputController();

    public event Action<ControllerAction> OnAction;

    private void Awake()
    {
        OnAction += OnControllerAction;
    }

    private void Update()
    {
        //для теста
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnAction?.Invoke(ControllerAction.boost);
        }
    }

    private void FixedUpdate()
    {
        var direction = controller.DesiredMoveDirection;

        var fixedsPerSecond = 1 / Time.fixedDeltaTime;

        body.AddForce(-body.velocity * SlowDownCoefficient * fixedsPerSecond + direction * MoveForce * Time.fixedDeltaTime);
    }

    private void OnControllerAction(ControllerAction action)
    {
        switch(action)
        {
            case ControllerAction.boost: 
                body.AddForce(controller.DesiredMoveDirection * BoostForce, ForceMode2D.Impulse); 
                break;
        }
    }

    public bool TryChangeController(in IController controller)
    {
        this.controller = controller;
        return true;
    }
}
