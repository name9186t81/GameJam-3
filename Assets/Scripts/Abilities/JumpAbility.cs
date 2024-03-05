using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAbility : AbilitiesContainer.Ability
{
    public override void update()
    {
        if(TryUse())
        {
            Debug.Log("da");
            //_player.
        }
    }
}
