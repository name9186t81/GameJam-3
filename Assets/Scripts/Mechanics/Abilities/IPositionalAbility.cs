using UnityEngine;

namespace Abilities
{
    public interface IPositionalAbility : IAbility
    {
        Vector2 WorldPosition { set; }
    }
}