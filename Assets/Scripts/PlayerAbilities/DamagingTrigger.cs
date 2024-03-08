using Core;
using Health;
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

        public void Init(float radius, float time, int team)
        {
            gameObject.SetActive(true);
            _collider.radius = radius;
            _teamNumber = team;
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

            if(raycast.TryGetComponent(out IDamageReactable act) && raycast.TryGetComponent(out ITeamProvider team))
            {
                if(team.TeamNumber != _teamNumber)
                {
                    if (act.CanTakeDamage(_damageArgs))
                    {
                        act.TakeDamage(_damageArgs);
                    }
                }
            }
        }
    }
}