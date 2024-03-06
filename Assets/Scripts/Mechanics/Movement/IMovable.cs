using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
    public interface IMovable
    {
        public Vector2 Position { get; }
        public float Rotation { get; set; }
        public Vector2 Velocity { get; set; }
    }
}