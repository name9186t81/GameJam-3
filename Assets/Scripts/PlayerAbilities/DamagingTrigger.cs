using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAbilities
{
    public class DamagingTrigger : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D _collider;

        public void Init(float radius, float time)
        {
            _collider.radius = radius;
            Destroy(gameObject, time);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            
        }
    }
}