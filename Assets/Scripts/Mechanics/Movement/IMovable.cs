using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
    public interface IMovable
    {
        public Vector2 Position { get; }
        public Vector2 Rotation { get; }
        public Vector2 Velocity { get; }
    }
}