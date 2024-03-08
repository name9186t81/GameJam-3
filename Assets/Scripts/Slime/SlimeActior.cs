using Core;
using GameLogic;
using Health;
using Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeActior : MonoBehaviour, IActor, IMovable, ITeamProvider //ITeamProvider пытается перебрасывать все с IHealth и нужен он тут потому что без скрипта одновременно с актором и провайдером ии еды не будет работать
{
    [SerializeField] private BoneJointsConnector _body;
    public SlimeHealth Health;
    [SerializeField] private float _baseMovevemntSpeed;
    [SerializeField] private float _baseRotationSpeed;
    [SerializeField] private float _playerHealthMult;

    public float CurrentScore => _body.CurrentScale * _body.CurrentScale;

    public Vector2 Position => _body.Position;
    public Vector2 Velocity { get { return _body.Velocity; } set { _body.Velocity = value; } }
    public float Rotation { get { return 0; } set { } }

    public IController Controller { get; private set; }
    public IActor Actor { get => this; set => throw new NotImplementedException(); }
    public int TeamNumber => Health.TeamNumber;

    public float Scale => Health.Radius;

    public event Action<ControllerAction> OnAction;
     
    public event Action<DamageArgs> OnDeath;
    public event Action<DamageArgs> OnDamage;
	public event Action OnInit;

    public event Action<int, int> OnTeamNumberChange;

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
        OnInit?.Invoke();
    }

    public bool TryChangeTeamNumber(int newTeamNumber)
    {
        OnTeamNumberChange?.Invoke(TeamNumber, newTeamNumber);
        return Health.TryChangeTeamNumber(newTeamNumber);
    }
}
