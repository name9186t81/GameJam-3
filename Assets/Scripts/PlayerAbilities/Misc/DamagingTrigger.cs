using Core;
using GameLogic;
using Health;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAbilities
{
    public class DamagingTrigger : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D _collider;
        [SerializeField] private int _damage;
        [SerializeField] private float _damageCooldownSeconds;

        private int _teamNumber;
        private DamageArgs _damageArgs;
        private bool _canMakeDamage = false;
        private int _lastFrameIndex = -1;
        private float _lastDamageMadeTime;
        private SlimeHealth _slimeHealth;
        private float _startRadius = -1;

        public void Init(float radius, float time, int team, SlimeHealth health)
        {
            if(_startRadius == -1)
                _startRadius = _collider.radius;

            gameObject.SetActive(true);
            transform.localScale = Vector3.one * radius / _startRadius;
            _teamNumber = team;
            _slimeHealth = health;
            _damageArgs = new DamageArgs(null, _damage, DamageFlags.Fire);
            Destroy(gameObject, time);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (_lastFrameIndex != Time.frameCount)
            {
                _lastFrameIndex = Time.frameCount;
                _canMakeDamage = Time.time - _lastDamageMadeTime > _damageCooldownSeconds;
                if(_canMakeDamage)
                {
                    _lastDamageMadeTime = Time.time;
                }
            }

            if (!_canMakeDamage)
                return;

            var raycast = collision.transform.root;

            if(raycast.TryGetComponent(out IDamageReactable act) && raycast.TryGetComponent(out ITeamProvider team) && raycast.TryGetComponent(out IProvider<IHealth> health))
            {
                if(team.TeamNumber != _teamNumber)
                {
                    Action<DamageArgs> onDeath = delegate (DamageArgs damageArgs) { _slimeHealth.OnEnemyKill(health.Value.MaxHealth); };
                    health.Value.OnDeath += onDeath;
                    if (act.CanTakeDamage(_damageArgs))
                    {
                        act.TakeDamage(_damageArgs);
                    }
                    health.Value.OnDeath -= onDeath;
                }
            }
        }
    }
}