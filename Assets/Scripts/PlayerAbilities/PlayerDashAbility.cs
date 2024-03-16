using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAbilities
{
    public class PlayerDashAbility : AbilitiesContainer.Ability
    {
        private protected override void update()
        {
            if (CanUse())
            {
                _player.Act(Core.ControllerAction.Dash);
            }
        }
    }
}