using Core;
using GameLogic;
using Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeActior : MonoBehaviour, IActor, IProvider<Motor>, IMovable
{
    [SerializeField] private BoneJointsConnector _body;
    [SerializeField] private float _baseMovevemntSpeed;
    [SerializeField] private float _baseRotationSpeed;

    public Motor Value { get; private set; }

    public Vector2 position => _body.position;
    public Vector2 velocity { get { return _body.velocity; } set { _body.velocity = value; } }
    public float rotation { get { return 0; } set { } }

    public IController Controller { get; private set; }

    public event Action<ControllerAction> OnAction;

    public bool TryChangeController(in IController controller)
    {
        if (Controller != null)
            Controller.OnAction -= Act;

        this.Controller = controller;
        controller.OnAction += Act;
        return true;
    }

    public void Act(ControllerAction obj)
    {
        OnAction?.Invoke(obj);
    }

    private void Awake()
    {
        Value = new Motor(_baseMovevemntSpeed, _baseRotationSpeed, this, this);
    }

    private void FixedUpdate()
    {
        Value.Update(Time.deltaTime); //по идее оно в фикседе должно быть потому что с рб работает
    }

    void Update()
    {
        
    }
}
