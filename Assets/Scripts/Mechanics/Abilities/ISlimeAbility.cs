using UnityEngine;

namespace Abilities
{
    public interface ISlimeAbility : IAbility
    {
        SlimeActior Slime { set; }
    }
}