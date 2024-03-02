using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneJointsConnector : MonoBehaviour
{
    [SerializeField] private float JointDamper = 0;
    [SerializeField] private float JointFrequency = 3;
    [SerializeField] private float ColliderRadius = 0.15f;
    [SerializeField] private float ColliderOffset = 0.0f;

    private void Awake()
    {
        var childs = GetComponentsInChildren<Rigidbody2D>();

        var half = childs.Length / 2;

        for (int i = 0; i < childs.Length; i++)
        {
            var current = childs[i];
            var next = childs[LoopIndex(i + 1, childs.Length)];
            var previous = childs[LoopIndex(i - 1, childs.Length)];
            var opposite = childs[LoopIndex(i + half, childs.Length)];

            Link(current, next);
            Link(current, previous);
            Link(current, opposite);

            var col = current.gameObject.AddComponent<CircleCollider2D>();
            col.offset = Vector2.right * ColliderOffset;
            col.radius = ColliderRadius;
        }

        Physics2D.SyncTransforms();
    }

    private void Link(Rigidbody2D a, Rigidbody2D b)
    {
        var joint = a.gameObject.AddComponent<SpringJoint2D>();
        joint.connectedBody = b;

        joint.dampingRatio = JointDamper;
        joint.frequency = JointFrequency;

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
