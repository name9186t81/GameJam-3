using UnityEngine;
using Core;
using System;
using GameLogic;
using PlayerInput;

public class PlayerController : MonoBehaviour, IController
{
    [SerializeField] private SlimeActior _playerSlime;

    private IActor _actor;
    public Vector2 DesiredMoveDirection { get; private set; }
    public Vector2 DesiredRotation => Vector2.up;

	public ControllerType Type => ControllerType.Player;
    public float CurrentScore => _playerSlime.CurrentScore;

	public event Action<ControllerAction> OnAction;

    private InputProvider _inputProvider;

    private void Awake()
    {
        _actor = GetComponent<IActor>();
        _actor.TryChangeController(this);
    }

    void Start()
    {
        _inputProvider = ServiceLocator.Get<InputProvider>();
        ServiceLocator.Get<PlayerTrackerService>().Bind(this);
    }

    private void Update()
    {
        DesiredMoveDirection = Vector2.ClampMagnitude(new Vector2(_inputProvider.Horizontal, _inputProvider.Vertical), 1);
    }
}
