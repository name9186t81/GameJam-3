using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Movement;
using Health;

namespace GameLogic
{
    public class SlimeHealth : MonoBehaviour, IHealth, IProvider<IHealth>, IDamageReactable, ITeamProvider
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

        IHealth IProvider<IHealth>.Value => this;

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
        public event SlimeCollision OnSlimeCollision;

        public delegate void SlimeCollision(SlimeHealth slime, BoneJointsConnector body, Vector2 collisionPoint);

        private float _startRadius = 1;
        private float _lastDamageHitTime;

        void Awake()
        {
            _body.OnCollisionEnter += OnBodyCollisionEnter;

            Actor = GetComponent<IActor>();

            _startRadius = _collider.radius;

            TryChangeTeamNumber(_startTeamNumber);

            OnInit?.Invoke();
        }

        private void onSlimeCollision(SlimeHealth slime, BoneJointsConnector body, Vector2 collisionPoint)
        {
            OnSlimeCollision?.Invoke(slime, body, collisionPoint);
        }

        private void OnBodyCollisionEnter(Collision2D collision)
        {
            var raycast = collision.collider.transform.root;

            if(raycast != transform && raycast.TryGetComponent(out SlimeHealth slime))
            {
                slime.onSlimeCollision(this, _body, collision.contacts[0].point);
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