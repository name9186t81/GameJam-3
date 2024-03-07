using Core;
using GameLogic;
using Health;
using Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeActior : MonoBehaviour, IActor, IProvider<Motor>, IMovable, IHealth
{
    [SerializeField] private BoneJointsConnector _body;
    [SerializeField] private float _baseMovevemntSpeed;
    [SerializeField] private float _baseRotationSpeed;
    [SerializeField] private float _playerHealthMult;

    public float CurrentScore => _body.CurrentScale * _body.CurrentScale;
    public Motor Value { get; private set; }

    public Vector2 Position => _body.Position;
    public Vector2 Velocity { get { return _body.Velocity; } set { _body.Velocity = value; } }
    public float Rotation { get { return 0; } set { } }

    public IController Controller { get; private set; }
    public IActor Actor { get => this; set => throw new NotImplementedException(); }
    public int CurrentHealth => Mathf.RoundToInt(CurrentScore * _playerHealthMult);
    public int MaxHealth => CurrentHealth;
    public HealthFlags Flags { get; set; }

    public event Action<ControllerAction> OnAction;

    public event Action<DamageArgs> OnDeath;
    public event Action<DamageArgs> OnDamage;

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

    public bool CanTakeDamage(DamageArgs args)
    {
        return true;
    }

    public void TakeDamage(DamageArgs args)
    {
        var direction = (args.HitPosition - args.SourcePosition).normalized;
        _body.TakeDamage(args.Damage / _playerHealthMult, args.HitPosition, direction);
    }
}
