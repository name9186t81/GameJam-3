using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : AbilitiesContainer.Ability
{
    [SerializeField] private float _reloadTime = 0.1f;

    private float _lastUseTime;

    public override void update()
    {
        if (Input.GetKeyDown(_useKey) && (Time.time - _lastUseTime) > _reloadTime)
        {
            _lastUseTime = Time.time;
            _player.Act(Core.ControllerAction.Dash);
        }
    }
}
