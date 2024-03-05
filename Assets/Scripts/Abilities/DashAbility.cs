using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : AbilitiesContainer.Ability
{
    public override void update()
    {
        if (TryUse())
        {
            _player.Act(Core.ControllerAction.Dash);
        }
    }
}
