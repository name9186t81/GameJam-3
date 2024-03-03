using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace GameLogic
{
    public class BoneJointsConnector : MonoBehaviour
    {
        [SerializeField] private SpringJointSettings EdgeJoint;
        [SerializeField] private SpringJointSettings StructuralJoint;
        [SerializeField] private float ColliderRadius = 0.15f;
        [SerializeField] private float ColliderOffset = 0.0f;
        [SerializeField] private AnimationCurve _forceToEdgeBonesMult;
        [SerializeField] private float _forceToEdgeBonesPow = 2f;
        [SerializeField] private bool _syncRootTransformPosition = true;

        [SerializeField] private float MinSize;
        [SerializeField] private float MaxSize;

        [HideInInspector] public Vector2 position;
        [HideInInspector] public Vector2 velocity; //�� ����������� ����� ��� ���������� ���� (���� � ������ ������ ��� ��� ���� ����������� ����� ����� �����������)
        [HideInInspector] public float Size { get { return currentSize; } set { SetSize(value); } }

        private Bone[] _bones;
        private bool _useInterpolation;
        private float currentSize = 0;
        private float currentScale => map(currentSize, 0, 1, MinSize, MaxSize);

        #region test

        [SerializeField] private float testSize;

        private void OnValidate()
        {
            if(Application.isPlaying && _bones != null)
                Size = testSize;
        }
        #endregion

        [System.Serializable] //����� ������ ���������� �� ��� �� ������������ �������� �� ��������� � ������ ������������ (�����)
        private class SpringJointSettings
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

            public readonly Vector2 startStrucuralSpringAnchor;

            public readonly SpringJoint2D structuralSpring;
            public readonly SpringJoint2D neighbourSpring1;
            public readonly SpringJoint2D neighbourSpring2;

            public void UpdateJoints(SpringJointSettings edges, SpringJointSettings structural, float currentSize)
            {
                edges.Apply(neighbourSpring1, currentSize);
                edges.Apply(neighbourSpring2, currentSize);
                structural.Apply(structuralSpring, currentSize);
            }

            public Bone(Rigidbody2D body, Vector2 startStrucuralSpringDistance, SpringJoint2D structuralSpring, SpringJoint2D neighbourSpring1, SpringJoint2D neighbourSpring2)
            {
                this.body = body;
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
            var bodies = GetComponentsInChildren<Rigidbody2D>();

            _bones = new Bone[bodies.Length];

            if (bodies.Length > 0)
                _useInterpolation = bodies[0].interpolation != RigidbodyInterpolation2D.None;

            var half = bodies.Length / 2;

            for (int i = 0; i < bodies.Length; i++)
            {
                var current = bodies[i];
                var next = bodies[LoopIndex(i + 1, bodies.Length)];
                var previous = bodies[LoopIndex(i - 1, bodies.Length)];
                var opposite = bodies[LoopIndex(i + half, bodies.Length)];

                var n1 = Link(current, next, EdgeJoint);
                var n2 = Link(current, previous, EdgeJoint);
                var structSpring = Link(current, opposite, StructuralJoint);

                AddBaseComponents(current);

                _bones[i] = new Bone(bodies[i], structSpring.connectedAnchor, structSpring, n1, n2);
            }
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
            position = Vector2.zero;
            velocity = Vector3.zero;

            for (int i = 0; i < _bones.Length; i++)
            {
                position += (Vector2)_bones[i].body.transform.position;
                velocity += _bones[i].body.velocity;
            }

            position /= _bones.Length;
            velocity /= _bones.Length;

            if (!_syncRootTransformPosition)
                return;

            var delta = (Vector3)position - transform.position;

            for (int i = 0; i < _bones.Length; i++)
            {
                _bones[i].body.transform.position -= delta;
            }

            transform.position += delta;
        }

        public void AddArea(float area)
        {
            area += currentScale * currentScale;
            var scale = Mathf.Sqrt(area);
            var size = map(scale, MinSize, MaxSize, 0, 1);
            SetSize(size);
        }

        private void SetSize(float newSize)
        {
            currentSize = newSize;

            for (int i = 0; i < _bones.Length; i++)
            {
                _bones[i].UpdateJoints(EdgeJoint, StructuralJoint, currentSize);
            }

            transform.localScale = Vector3.one * currentScale;
        }

        public void AddForceToAll(Vector3 force, ForceMode2D forceMode = ForceMode2D.Force)
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                _bones[i].body.AddForce(force, forceMode);
            }
        }
        
        public void AddForce(Vector3 force, ForceMode2D forceMode = ForceMode2D.Force)
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                var vec = position - (Vector2)_bones[i].body.transform.position;
                var mult = Mathf.Pow(1 - Mathf.Abs(Vector2.Dot(force.normalized, vec.normalized)), _forceToEdgeBonesPow);
                //bodies[i].AddForce(force * map(mult, 0, 1, _forceToEdgeBonesMult, 1), forceMode);
                _bones[i].body.AddForce(force, forceMode);
                //_bones[i].structuralSpring.connectedAnchor = _bones[i].startStrucuralSpringAnchor * map(mult, 0, 1, _forceToEdgeBonesMult, 1);
                _bones[i].body.AddForce(vec.normalized * force.magnitude * _forceToEdgeBonesMult.Evaluate(currentSize) * mult, forceMode);
            }
        }

        //���� ����� ���������� ������
        private float map(float X, float A, float B, float C, float D)
        {
            return (X - A) / (B - A) * (D - C) + C;
        }

        private void AddBaseComponents(Rigidbody2D obj)
        {
            var col = obj.gameObject.AddComponent<CircleCollider2D>();
            col.offset = Vector2.right * ColliderOffset;
            col.radius = ColliderRadius;
        }

        private SpringJoint2D Link(Rigidbody2D a, Rigidbody2D b, SpringJointSettings s)
        {
            var joint = a.gameObject.AddComponent<SpringJoint2D>();
            joint.connectedBody = b;

            s.Apply(joint, currentSize);

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