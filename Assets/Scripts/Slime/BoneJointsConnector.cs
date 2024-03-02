using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class BoneJointsConnector : MonoBehaviour
{
    [SerializeField] private GenerationMethod generationMethod;
    [SerializeField] private SpringJointSettings EdgeJoint;
    [SerializeField] private SpringJointSettings StructuralJoint;
    [SerializeField] private float ColliderRadius = 0.15f;
    [SerializeField] private float ColliderOffset = 0.0f;

    public enum GenerationMethod
    {
        NeighboursAndOpposite,
        AllToAll //попробовал для теста, говно
    }

    [System.Serializable] //хотел делать структурой но она не поддерживает значения по умолчанию и пустые конструкторы (говно)
    private class SpringJointSettings
    {
        [Range(0.0f, 1.0f)]
        public float JointDamper = 0;
        public float JointFrequency = 3;
    }

    [InspectorButton(nameof(generateRigidbodies))]
    [SerializeField] private bool GenerateRigidbodies;

    private void generateRigidbodies()
    {
        var childs = GetComponentsInChildren<Transform>();

        for (int i = 0; i < childs.Length; i++)
            if(childs[i] != transform && !childs[i].TryGetComponent(out Rigidbody2D b))
                childs[i].gameObject.AddComponent<Rigidbody2D>();
    }

    private void OnValidate()
    {
        if(!TryGetComponent(out SpriteSkin skin))
        {
            //удаляем трансформы которые насрал SpriteSkin при его пересоздании
            var childs = GetComponentsInChildren<Transform>();

            foreach (var child in childs)
                if (child != transform)
                    Destroy(child.gameObject);
        }
    }

    private void Awake()
    {
        var childs = GetComponentsInChildren<Rigidbody2D>();

        switch (generationMethod)
        {
            case GenerationMethod.NeighboursAndOpposite:
                {
                    var half = childs.Length / 2;

                    for (int i = 0; i < childs.Length; i++)
                    {
                        var current = childs[i];
                        var next = childs[LoopIndex(i + 1, childs.Length)];
                        var previous = childs[LoopIndex(i - 1, childs.Length)];
                        var opposite = childs[LoopIndex(i + half, childs.Length)];

                        Link(current, next, EdgeJoint);
                        Link(current, previous, EdgeJoint);
                        Link(current, opposite, StructuralJoint);

                        AddBaseComponents(current);
                    }
                    break;
                }
            case GenerationMethod.AllToAll:
                {
                    for (int i = 0; i < childs.Length; i++)
                    {
                        var current = childs[i];
                        AddBaseComponents(current);

                        for (int j = 0; j < childs.Length; j++)
                        {
                            Link(current, childs[j], StructuralJoint);
                        }
                    }
                    break;
                }
        }
    }

    private void AddBaseComponents(Rigidbody2D obj)
    {
        var col = obj.gameObject.AddComponent<CircleCollider2D>();
        col.offset = Vector2.right * ColliderOffset;
        col.radius = ColliderRadius;
    }

    private void Link(Rigidbody2D a, Rigidbody2D b, SpringJointSettings s)
    {
        var joint = a.gameObject.AddComponent<SpringJoint2D>();
        joint.connectedBody = b;

        joint.dampingRatio = s.JointDamper;
        joint.frequency = s.JointFrequency;

        //думаю что что-то из этого можно убрать но мне лень
        joint.autoConfigureConnectedAnchor = true;
        joint.autoConfigureConnectedAnchor = false;
        joint.autoConfigureDistance = true;
        joint.autoConfigureDistance = false;
    }

    private int LoopIndex(int index, int count)
    {
        return (int)Mathf.Repeat(index, count);
    }
}
