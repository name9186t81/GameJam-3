using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAbilities
{
    public class JumpPointSelector : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private float _spriteRadius = 1;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _restrictedColor;
        [SerializeField] private float _colorSmoothTime = 0.1f;
        [SerializeField] private LayerMask _overlapLayers = -1;
        [SerializeField] private float _radiusMult = 0.6f;

        public Func<Vector2> PositionProvider;

        public bool CanJump { get; private set; } = true;

        private FloatSmoothDamp _smoothDamp;
        private float _currentRadius;

        private void Awake()
        {
            _currentRadius = _spriteRadius;
            _smoothDamp = new FloatSmoothDamp(_colorSmoothTime, 0);
        }

        public void SetRadius(float radius)
        {
            radius *= _radiusMult;

            _currentRadius = radius;
            transform.localScale = Vector3.one * (radius / _spriteRadius);

            Update();
        }

        private bool DoOverlap()
        {
            return Physics2D.OverlapCircle(transform.position, _currentRadius, _overlapLayers);
        }

        public void Update()
        {
            if(PositionProvider != null)
                transform.position = PositionProvider();
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);

            CanJump = !DoOverlap();

            var state = _smoothDamp.Update(CanJump ? 0 : 1, Time.unscaledDeltaTime);
            _sprite.color = Color.Lerp(_normalColor, _restrictedColor, state);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _spriteRadius);
        }
    }
}