using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace abilities
{
    public class PlayerJumpAbility : AbilitiesContainer.Ability
    {
        private protected override void update()
        {
            if (TryUse())
            {
                Debug.Log("da");
                //_player.
            }
        }
    }
}