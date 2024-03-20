using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAbilities
{
    public class PlayerDashAbility : AbilitiesContainer.Ability
    {
        public override void Use()
        {
            throw new System.NotImplementedException();
        }

        private protected override void update(float dt)
        {
            if (CanUse())
            {
                //_actor.Act(Core.ControllerAction.Dash);
            }
        }
    }
}