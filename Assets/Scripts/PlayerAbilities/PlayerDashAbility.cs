using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace abilities
{
    public class PlayerDashAbility : AbilitiesContainer.Ability
    {
        private protected override void update()
        {
            if (TryUse())
            {
                _player.Act(Core.ControllerAction.Dash);
            }
        }
    }
}