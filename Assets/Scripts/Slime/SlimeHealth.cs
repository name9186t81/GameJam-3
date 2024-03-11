using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Movement;
using Health;

namespace GameLogic
{
    public class SlimeHealth : MonoBehaviour, IProvider<Motor>, IMovable, IHealth, IProvider<IHealth>, IDamageReactable, ITeamProvider
    {
        [SerializeField] private BoneJointsConnector _body;
        [Header("Health and damage")]
        [SerializeField] private AnimationCurve _incomingDamageMultOverSize;
        [SerializeField] private float _playerHealthMult;
        [SerializeField] private AnimationCurve _damageMultOverSize;
        [SerializeField] private float _healthCanBeEatedPerScale = 30;
        [SerializeField] private int _hitDamage;
        [SerializeField] private float _hitDamageCooldownSeconds = 0.1f;
        [SerializeField] private float _areaPerHealthDiv = 100f;
        [SerializeField] private AnimationCurve _scoreMultPerCombo;
        [SerializeField] private int _startTeamNumber;
        [SerializeField] private CircleCollider2D _collider;
        [SerializeField] private float _slowdownFactor = 75;

        [SerializeField] private bool _useMotor = false;
        [SerializeField] private float _baseMovementSpeed;
        [SerializeField] private float _baseRotationSpeed;

        IHealth IProvider<IHealth>.Value => this;
        public Motor Value { get; private set; }

        public Vector2 Position { get => _body.Position; set { _body.Position = value; } }
        public Vector2 Velocity { get { return _body.Velocity; } set { _body.AddForce(value); } } //ага да вот текущая скорость да да конечно скорость будет как ты хочешь мотор
        public float Rotation { get => 0; set { } }

        public event Action<int, int> OnTeamNumberChange;
        public event Action<DamageArgs> OnDeath;
        public event Action<DamageArgs> OnDamage;
        public event Action OnInit;
        public int TeamNumber { get; private set; }

        public float CurrentScore => _body.Scale * _body.Scale;
        public IActor Actor { get; set; }
        public int CurrentHealth => Mathf.RoundToInt(CurrentScore * _playerHealthMult / _incomingDamageMultOverSize.Evaluate(_body.Size));
        public int MaxHealth => CurrentHealth;

        public float Radius => _startRadius * _body.Scale;

        public HealthFlags Flags { get; set; } = HealthFlags.FriendlyFireDisabled;
        public event Action<float> OnAddScore;

        private float _startRadius = 1;
        private float _lastDamageHitTime;

        void Awake()
        {
            _body.OnCollisionEnter += OnBodyCollisionEnter;

            Actor = GetComponent<IActor>();

            _startRadius = _collider.radius;

            Value = new Motor(_baseMovementSpeed, _baseRotationSpeed, this, Actor);
            /*
             * R.I.P. Костыль
             * 06.03.2024 - 08.03.2024
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

            TryChangeTeamNumber(_startTeamNumber);

            OnInit?.Invoke();
        }

        private void FixedUpdate()
        {
            _body.AddForceToAll(-_body.Velocity * _slowdownFactor * Time.fixedDeltaTime);

            if (_useMotor)
                Value.Update(Time.deltaTime);
        }

        private void OnBodyCollisionEnter(Collision2D collision)
        {
            var raycast = collision.collider.transform.root;

            if(raycast != transform && raycast.TryGetComponent(out PlayerActor player))
            {
                player.OnSlimeCollision(this, _body, collision.contacts[0].point);
            }
            else if (raycast.TryGetComponent(out IDamageReactable act) && raycast.TryGetComponent(out IProvider<IHealth> healthProvider) && healthProvider.Value.CurrentHealth > 0)
            {
                if (raycast.TryGetComponent<ITeamProvider>(out var team) && team.TeamNumber == TeamNumber)
                    return;

                var health = healthProvider.Value;

                var canBeEated = health.MaxHealth < _body.Scale * _healthCanBeEatedPerScale;

                int damage = 0;

                if (canBeEated)
                {
                    damage = health.MaxHealth + 1; //наверное не нужно +1 но на случай если еда сделает проверку не правильную!!
                }
                else
                {
                    damage = (Time.time - _lastDamageHitTime > _hitDamageCooldownSeconds) ? _hitDamage : 0;

                    if(damage != 0) //мне лень
                    {
                        _lastDamageHitTime = Time.time;
                    }
                }

                var _damageArgs = new DamageArgs(Actor, damage, DamageFlags.Melee, null, Radius);

                Action<DamageArgs> onDeath = delegate (DamageArgs args)
                {
                    OnEnemyKill(health.MaxHealth);
                };

                health.OnDeath += onDeath;

                bool check = act.CanTakeDamage(_damageArgs);

                if (check)
                {
                    _damageArgs.HitPosition = collision.contacts[0].point;
                    act.TakeDamage(_damageArgs);
                }
                else
                {
                    Debug.LogError("why");
                }

                health.OnDeath -= onDeath;
            }
        }

        public void OnEnemyKill(float maxHealth)
        {
            AddScore(maxHealth / _areaPerHealthDiv);
        }

        public void AddScore(float score)
        {
            score = score * _scoreMultPerCombo.Evaluate(ComboUI.ComboCount);
            _body.AddArea(score);
            OnAddScore?.Invoke(score);
        }

        public bool CanTakeDamage(DamageArgs args)
        {
            return true;
        }

        public void TakeDamage(DamageArgs args)
        {
            var direction = (args.HitPosition - args.SourcePosition).normalized;

            if (!_body.TakeDamage((args.Damage / _playerHealthMult) * _damageMultOverSize.Evaluate(_body.Size), args.HitPosition, direction))
            {
                OnDeath?.Invoke(args);
            }
        }

        public bool TryChangeTeamNumber(int newTeamNumber)
        {
            OnTeamNumberChange?.Invoke(TeamNumber, newTeamNumber);
            TeamNumber = newTeamNumber;
            return true;
        }
    }
}