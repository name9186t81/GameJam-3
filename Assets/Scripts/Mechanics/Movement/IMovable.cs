using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
    public interface IMovable
    {
        public Vector2 position { get; }
        public float rotation { get; set; }
        public Vector2 velocity { get; set; }
    }
}