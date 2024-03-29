using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace GameLogic
{
    public class BoneJointsConnector : MonoBehaviour
    {
        [SerializeField] private PhysicsMaterial2D _physicsMaterial;
        [SerializeField] private SpringJointSettings EdgeJoint;
        [SerializeField] private SpringJointSettings StructuralJoint;
        [SerializeField] private float ColliderRadius = 0.15f;
        [SerializeField] private float ColliderOffset = 0.0f;
        [SerializeField] private AnimationCurve _forceToEdgeBonesMult;
        [SerializeField] private float _forceToEdgeBonesPow = 2f;
        [SerializeField] private Transform _transformToMove;
        [SerializeField] private bool _syncTransformPosition = false;
        [SerializeField] private AnimationCurve _massOverSize;
        [SerializeField] private float _massOverSizeMult = 10;

        [SerializeField] private float _damageForceMult = 10;

        [SerializeField] private float _minSize = 1;
        [SerializeField] private float _maxSize = 15;
        [SerializeField] private float _deathScaleThreshold = 0.1f;
        [SerializeField] private float _startBodySize = 0;
        private float _deathSizeThreshold;

        [SerializeField] private CircleCollider2D _circleColliderToMove;

        public Vector2 TransformPosition => _transformPosition;
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                var delta = value - _position;

                if (_ignoreCollisions) //��� ����
                {
                    transform.position += (Vector3)delta;
                    Velocity = Vector2.zero;

                    for (int i = 0; i < _bones.Length; i++)
                    {
                        _bones[i].body.position += delta;
                    }
                }
                else
                {
                    for (int i = 0; i < _bones.Length; i++)
                    {
                        _bones[i].body.position += delta;
                    }
                }
                _position = value;
            }
        }
        public Vector2 Velocity {
            get { return _velocity; }
            set
            {
                var delta = value - _velocity;
                for (int i = 0; i < _bones.Length; i++)
                {
                    _bones[i].body.velocity += delta;
                }
                _velocity = value;
            }
        } //�� ����������� ����� ��� ���������� ���� (���� � ������ ������ ��� ��� ���� ����������� ����� ����� �����������)
        private Vector2 _velocity;
        private Vector2 _position;
        private Vector2 _transformPosition;
        private bool _ignoreCollisions = false;
        private Vector2 _colliderBaseOffset;

        [HideInInspector] public float Size { get { return _currentSize; } set { SetSize(value); } }

        public event System.Action<Collision2D> OnCollisionEnter;
        public event System.Action<Collision2D> OnCollisionStay;
        public event System.Action<Collision2D> OnCollisionExit;

        public event System.Action<float> OnSizeChanged;
        public float Scale { get { return Size.map01(_minSize, _maxSize); } set { Size = value.map(_minSize, _maxSize, 0, 1); } }

        private Bone[] _bones;
        private bool _useInterpolation;
        private float _currentSize = 0;
        private float _currentMass = 1;

        #region test

        [Range(0, 1)]
        [SerializeField] private float testSize;

        private void OnValidate()
        {
            if(Application.isPlaying && _bones != null)
                Size = testSize;
        }
        #endregion

        [System.Serializable]
        public class SpringJointSettings
        {
            [SerializeField] private float _damperMult = 0;
            [SerializeField] private float _frequencyMult = 3;

            [SerializeField] private AnimationCurve _damperOverSize;
            [SerializeField] private AnimationCurve _frequencyOverSize;

            public void Apply(SpringJoint2D joint, float currentSize)
            {
                joint.dampingRatio = _damperOverSize.Evaluate(currentSize) * _damperMult;
                joint.frequency = _frequencyOverSize.Evaluate(currentSize) * _frequencyMult;
            }
        }

        private struct Bone
        {
            public readonly Rigidbody2D body;
            public readonly Collider2D collider;

            public readonly Vector2 startStrucuralSpringAnchor;

            public readonly SpringJoint2D structuralSpring;
            public readonly SpringJoint2D neighbourSpring1;
            public readonly SpringJoint2D neighbourSpring2;

            public void UpdateSize(SpringJointSettings edges, SpringJointSettings structural, float currentSize, float mass)
            {
                edges.Apply(neighbourSpring1, currentSize);
                edges.Apply(neighbourSpring2, currentSize);
                structural.Apply(structuralSpring, currentSize);
                body.mass = mass;
            }

            public Bone(Rigidbody2D body, Collider2D col, Vector2 startStrucuralSpringDistance, SpringJoint2D structuralSpring, SpringJoint2D neighbourSpring1, SpringJoint2D neighbourSpring2)
            {
                this.body = body;
                this.collider = col;
                this.startStrucuralSpringAnchor = startStrucuralSpringDistance;
                this.structuralSpring = structuralSpring;
                this.neighbourSpring1 = neighbourSpring1;
                this.neighbourSpring2 = neighbourSpring2;
            }
        }

        [InspectorButton(nameof(GenerateRigidbodies))]
        [SerializeField] private bool _generateRigidbodies;

        private void GenerateRigidbodies()
        {
            var childs = GetComponentsInChildren<Transform>();

            for (int i = 0; i < childs.Length; i++)
                if (childs[i] != transform && !childs[i].TryGetComponent(out Rigidbody2D b))
                    childs[i].gameObject.AddComponent<Rigidbody2D>();
        }

        private void Awake()
        {
            if(_circleColliderToMove != null)
                _colliderBaseOffset = _circleColliderToMove.offset;

            _deathSizeThreshold = _deathScaleThreshold.map(_minSize, _maxSize, 0, 1);

            var bodies = GetComponentsInChildren<Rigidbody2D>();

            _bones = new Bone[bodies.Length];

            if (bodies.Length > 0)
                _useInterpolation = bodies[0].interpolation != RigidbodyInterpolation2D.None;

            var half = bodies.Length / 2;

            for (int i = 0; i < bodies.Length; i++)
            {
                bodies[i].sharedMaterial = _physicsMaterial;
                var current = bodies[i];
                var next = bodies[LoopIndex(i + 1, bodies.Length)];
                var previous = bodies[LoopIndex(i - 1, bodies.Length)];
                var opposite = bodies[LoopIndex(i + half, bodies.Length)];

                var n1 = Link(current, next, EdgeJoint);
                var n2 = Link(current, previous, EdgeJoint);
                var structSpring = Link(current, opposite, StructuralJoint);

                var col = AddBaseComponents(current);

                _bones[i] = new Bone(bodies[i], col, structSpring.connectedAnchor, structSpring, n1, n2);
            }

            for (int i = 0; i < bodies.Length; i++)
            {
                for (int j = 0; j < bodies.Length; j++)
                {
                    Physics2D.IgnoreCollision(_bones[i].collider, _bones[j].collider, true);
                }
            }

            Size = _startBodySize;
        }

        private void Update()
        {
            if(_useInterpolation)
                UpdateData();
        }

        private void FixedUpdate()
        {
            if (!_useInterpolation)
                UpdateData();
        }

        private void UpdateData()
        {
            _position = Vector2.zero;
            _velocity = Vector3.zero;
            _transformPosition = Vector3.zero;

            for (int i = 0; i < _bones.Length; i++)
            {
                _position += (Vector2)_bones[i].body.position;
                _transformPosition += (Vector2)_bones[i].body.transform.position;
                _velocity += _bones[i].body.velocity;
            }

            _position /= _bones.Length;
            _transformPosition /= _bones.Length;
            _velocity /= _bones.Length;

            if (!_syncTransformPosition)
                return;

            _transformToMove.position = _transformPosition;

            if (_circleColliderToMove != null)
                _circleColliderToMove.offset = (Vector2)transform.InverseTransformPoint(_transformPosition) + _colliderBaseOffset;
        }

        public void SetIgnoreWorldCollision(bool ignore)
        {
            _ignoreCollisions = ignore;

            var layers = ignore ? -1 : 0;

            for (int i = 0; i < _bones.Length; i++)
            {
                _bones[i].collider.excludeLayers = layers;
            }
        }

        public bool TakeDamage(float damage, Vector2 position, Vector2 direction)
        {
            AddArea(-damage);

            bool tookDamage = true;

            if(_currentSize <= _deathSizeThreshold)
            {
                tookDamage = false;
            }

            //� ������ �� ����� ����� ������ � ���������� � ����������� �� ����� � ������� ������ ������ ������ �� ����
            AddForceToNearestBone(position, direction * damage * _damageForceMult);

            return tookDamage;
        }

        public void AddForceToNearestBone(Vector2 position, Vector3 force)
        {
            float closestDist = float.MaxValue;
            int closestBone = -1;

            for (int i = 0; i < _bones.Length; i++)
            {
                var dist = Vector2.Distance(_bones[i].body.position, position);

                if (dist < closestDist)
                {
                    closestBone = i;
                    closestDist = dist;
                }
            }

            _bones[closestBone].body.AddForce(force, ForceMode2D.Impulse);

        }

        public void AddArea(float area)
        {
            area += Scale * Scale;
            var scale = Mathf.Sqrt(area);
            var size = scale.map(_minSize, _maxSize, 0, 1);
            SetSize(size);
        }

        private void SetSize(float newSize)
        {
            //UpdateData();

            testSize = newSize;

            if (float.IsNaN(newSize))
                newSize = _deathSizeThreshold;

            _currentSize = Mathf.Max(_deathSizeThreshold, newSize);
            _currentMass = _massOverSize.Evaluate(newSize) * _massOverSizeMult;

            for (int i = 0; i < _bones.Length; i++)
            {
                _bones[i].UpdateSize(EdgeJoint, StructuralJoint, _currentSize, _currentMass);
            }

            var pos = _transformToMove.position;

            var delta = Scale / transform.localScale.x;
            transform.localScale *= delta;

            transform.position -= _transformToMove.position - pos;

            OnSizeChanged?.Invoke(newSize);
        }

        public void AddForceToAll(Vector3 force, ForceMode2D forceMode = ForceMode2D.Force)
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                _bones[i].body.AddForce(force * _currentMass, forceMode);
            }
        }
        
        public void AddForce(Vector3 force, ForceMode2D forceMode = ForceMode2D.Force)
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                var vec = Position - (Vector2)_bones[i].body.transform.position;
                var mult = Mathf.Pow(1 - Mathf.Abs(Vector2.Dot(force.normalized, vec.normalized)), _forceToEdgeBonesPow);
                //bodies[i].AddForce(force * map(mult, 0, 1, _forceToEdgeBonesMult, 1), forceMode);
                _bones[i].body.AddForce(force * _currentMass, forceMode);
                //_bones[i].structuralSpring.connectedAnchor = _bones[i].startStrucuralSpringAnchor * map(mult, 0, 1, _forceToEdgeBonesMult, 1);
                _bones[i].body.AddForce(vec.normalized * force.magnitude * _forceToEdgeBonesMult.Evaluate(_currentSize) * mult * _currentMass, forceMode);
            }
        }

        //���� ����� ���������� ������

        private CircleCollider2D AddBaseComponents(Rigidbody2D obj)
        {
            var col = obj.gameObject.AddComponent<CircleCollider2D>();
            col.offset = Vector2.right * ColliderOffset;
            col.radius = ColliderRadius;

            var callbacks = obj.gameObject.AddComponent<CollisionCallbackRethrow>();
            callbacks.OnCollisionEnter += _onCollisionEnter;
            callbacks.OnCollisionStay += _onCollisionStay;
            callbacks.OnCollisionExit += _onCollisionExit;

            return col;
        }

        private void _onCollisionEnter(Collision2D collision) => OnCollisionEnter?.Invoke(collision);

        private void _onCollisionStay(Collision2D collision) => OnCollisionStay?.Invoke(collision);

        private void _onCollisionExit(Collision2D collision) => OnCollisionExit?.Invoke(collision);

        public SpringJoint2D GrabObject(Rigidbody2D obj, Vector2 direction, SpringJointSettings s, Vector2 anchor)
        {
            int nearestBoneInd = -1;
            float maxDot = -1;

            for (int i = 0; i < _bones.Length; i++)
            {
                var vec = (_bones[i].body.position - Position).normalized;
                var dot = Vector2.Dot(direction, vec);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    nearestBoneInd = i;
                }
            }

            var secondBody = _bones[nearestBoneInd].body;

            var joint = Link(obj, secondBody, s);

            joint.distance = 0;
            joint.connectedAnchor = Vector2.zero;
            joint.anchor = anchor;

            return joint;
        }

        private SpringJoint2D Link(Rigidbody2D a, Rigidbody2D b, SpringJointSettings s)
        {
            var joint = a.gameObject.AddComponent<SpringJoint2D>();
            joint.connectedBody = b;

            s.Apply(joint, _currentSize);

            //����� ��� ���-�� �� ����� ����� ������ �� ��� ����
            joint.autoConfigureConnectedAnchor = true;
            joint.autoConfigureConnectedAnchor = false;
            joint.autoConfigureDistance = true;
            joint.autoConfigureDistance = false;

            return joint;
        }

        private int LoopIndex(int index, int count)
        {
            return (int)Mathf.Repeat(index, count);
        }
    }
}