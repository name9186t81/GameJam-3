using UnityEngine;

namespace Movement
{
    //������� ���� ����� ���� ��������� ���������� ��� ������� � �����������
    public interface IMovable
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Velocity { get; set; }
    }
}