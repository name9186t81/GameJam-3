using UnityEngine;

namespace Movement
{
    //дорогой хмук прошу теб€ соблюдать ѕаскал ейс дл€ свойств в интерфейсах
    public interface IMovable
    {
        public Vector2 Position { get; }
        public float Rotation { get; set; }
        public Vector2 Velocity { get; set; }
    }
}