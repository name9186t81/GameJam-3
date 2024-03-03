using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System;

public class PlayerInputController : IController
{
    public Vector2 DesiredMoveDirection => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

    public Vector2 DesiredRotation => throw new NotImplementedException();

    public event Action<ControllerAction> OnAction;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            //todo
        }
    }
}
