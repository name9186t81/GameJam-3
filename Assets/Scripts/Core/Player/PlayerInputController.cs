using UnityEngine;
using Core;
using System;
using GameLogic;
using PlayerInput;

public class PlayerInputController : MonoBehaviour, IController
{
    [SerializeField] private PlayerActor _player;
    public Vector2 DesiredMoveDirection { get; private set; }
    public Vector2 DesiredRotation => Vector2.up;

	public ControllerType Type => ControllerType.Player;

	public event Action<ControllerAction> OnAction;

    private InputProvider _inputProvider;

    private void Awake()
    {
        _player.TryChangeController(this);
    }

    void Start()
    {
        _inputProvider = ServiceLocator.Get<InputProvider>();
    }

    private void Update()
    {
        DesiredMoveDirection = new Vector2(_inputProvider.Horizontal, _inputProvider.Vertical);
    }
}
