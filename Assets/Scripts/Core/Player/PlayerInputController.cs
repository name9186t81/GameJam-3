using UnityEngine;
using Core;
using System;
using GameLogic;

public class PlayerInputController : MonoBehaviour, IController
{
    [SerializeField] private PlayerActor _player;
    public Vector2 DesiredMoveDirection { get; private set; }
    public Vector2 DesiredRotation => Vector2.up;

	public ControllerType Type => ControllerType.Player;

	public event Action<ControllerAction> OnAction;

    private void Awake()
    {
        _player.TryChangeController(this);
    }

    private void Update()
    {
        DesiredMoveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
