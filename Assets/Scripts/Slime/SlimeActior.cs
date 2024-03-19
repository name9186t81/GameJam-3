using Core;
using GameLogic;
using Health;
using Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeActior : MonoBehaviour, IProvider<Motor>, IActor, IMovable, ITeamProvider, IProvider<IHealth> //ITeamProvider пытается перебрасывать все с IHealth и нужен он тут потому что без скрипта одновременно с актором и провайдером ии еды не будет работать
{
    [SerializeField] private BoneJointsConnector _body;
    [SerializeField] private SlimeHealth _health;

    [SerializeField] private float _baseMovementSpeed = 5000;
    [SerializeField] private float _baseRotationSpeed = 1000;
    [SerializeField] private float _slowdownFactor = 75;
    [SerializeField] private float _slimeMergeForcePerScore;

    [SerializeField] private ComboCounter _comboCounter;

    public float CurrentScore => _body.Scale * _body.Scale;

    public Vector2 Position { get => _body.Position; set { _body.Position = value; } }
    Vector2 IActor.Position { get => _body.TransformPosition; } //da
    public Vector2 TransformPosition { get => _body.TransformPosition; }
    public Vector2 Velocity { get { return _body.Velocity; } set { _body.AddForce(value * _comboCounter.Config.SpeedMultipler.GetValue(_comboCounter.ComboCount)); } } //ага да вот текущая скорость да да конечно скорость будет как ты хочешь мотор
    public float Rotation { get { return 0; } set { } }

    public IController Controller { get; private set; }
    public IActor Actor { get => this; set => throw new NotImplementedException(); }
    public int TeamNumber => _health.TeamNumber;

    public float Size => _body.Size;
    public float Scale => _health.Radius;
    float IActor.Scale => _health.Radius;
    public float Radius => _health.Radius;

    public string Name => "Slime";

    public IHealth Value => _health;
    Motor IProvider<Motor>.Value => _motor;
    public SlimeHealth Health => _health;
    public ComboCounter ComboCounter => _comboCounter;

    public event Action<ControllerAction> OnAction;
     
    public event Action<DamageArgs> OnDeath;
    public event Action<DamageArgs> OnDamage;
	public event Action OnInit;

    public event Action<int, int> OnTeamNumberChange;

    private Motor _motor;

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
        _health.OnSlimeCollision += OnSlimeCollision;
        _health.OnAddScore += OnAddScore;

        _motor = new Motor(_baseMovementSpeed * Time.fixedDeltaTime, _baseRotationSpeed, this, Actor);
        /*
         * R.I.P. Костыль
         * 06.03.2024 - 08.03.2024
         * костыль был перезахоронен 19.03.2024
           _        _
          ( `-.__.-' )
           `-.    .-'
              \  /
               ||
               ||
              //\\
             //  \\
            ||    ||
            ||____||
            ||====||
             \\  //
              \\//
               ||
               ||
               ||
               ||
               ||
               ||
               ||
               ||
               []
        */
    }

    public void OnAddScore(float score)
    {
        _comboCounter.OnKill();
    }

    void FixedUpdate()
    {
        _body.AddForceToAll(-_body.Velocity * _slowdownFactor * Time.fixedDeltaTime);
        _motor.Update(Time.deltaTime);
    }

    void Update()
    {
        _comboCounter.Update(Time.deltaTime);
    }

    public bool TryChangeTeamNumber(int newTeamNumber)
    {
        OnTeamNumberChange?.Invoke(TeamNumber, newTeamNumber);
        return _health.TryChangeTeamNumber(newTeamNumber);
    }

    public void OnSlimeCollision(SlimeHealth slime, BoneJointsConnector body, Vector2 collisionPoint)
    {
        Debug.LogError("TODO");
        //if(PlayerPetSlimesAbility.TryReturnSlime(body, _body))
        {
            _body.AddForceToNearestBone(collisionPoint, (body.Position - Position).normalized * _slimeMergeForcePerScore * CurrentScore);
            Destroy(slime.gameObject); //прощай слаймик!!
        }
    }
}
