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
        [SerializeField] private IActor _actor;
        [Header("Health and damage")]
        [SerializeField] private float _playerHealthMult;
        [SerializeField] private AnimationCurve _damageMultOverSize;
        [SerializeField] private float _healthCanBeEatedPerScale = 30;
        [SerializeField] private int _hitDamage;
        [SerializeField] private float _areaPerHealthDiv = 100f;
        [SerializeField] private AnimationCurve _scoreMultPerCombo;
        [SerializeField] private int _startTeamNumber;

        IHealth IProvider<IHealth>.Value => this;
        public Motor Value { get; private set; }

        public Vector2 Position { get => _body.Position; set { _body.Position = value; } }
        public Vector2 Velocity { get => _body.Velocity; set { _body.Velocity = value; } }
        public float Rotation { get => 0; set { } }

        public event Action<int, int> OnTeamNumberChange;
        public event Action<DamageArgs> OnDeath;
        public event Action<DamageArgs> OnDamage;
        public event Action OnInit;
        public int TeamNumber { get; private set; }

        public float CurrentScore => _body.CurrentScale * _body.CurrentScale;
        public IActor Actor { get => _actor; set => _actor = value; }
        public int CurrentHealth => Mathf.RoundToInt(CurrentScore * _playerHealthMult);
        public int MaxHealth => CurrentHealth;
        public HealthFlags Flags { get; set; } = HealthFlags.FriendlyFireDisabled;
        public event Action<float> OnAddScore;

        void Awake()
        {
            _body.OnCollisionEnter += OnBodyCollisionEnter;

            Value = new Motor(0, 0, this, _actor); //0 ������ ��� ���� ��������� �� ����� ����� � ����� ����� ��� ������� ������
            /*
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

        private void OnBodyCollisionEnter(Collision2D collision)
        {
            var raycast = collision.collider.transform;

            if (raycast.TryGetComponent<IDamageReactable>(out IDamageReactable act) && raycast.TryGetComponent(out IProvider<IHealth> healthProvider) && healthProvider.Value.CurrentHealth > 0)
            {
                var health = healthProvider.Value;

                var canBeEated = health.MaxHealth < _body.CurrentScale * _healthCanBeEatedPerScale;

                int damage = 0;

                if (canBeEated)
                {
                    damage = health.MaxHealth + 1; //da
                }
                else
                {
                    damage = _hitDamage;
                }

                var _damageArgs = new DamageArgs(_actor, damage, DamageFlags.Melee);

                Action<DamageArgs> onDeath = delegate (DamageArgs args)
                {
                    AddScore(health.MaxHealth / _areaPerHealthDiv);
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
            var direction = (args.HitPosition - args.Sender.Position).normalized;

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