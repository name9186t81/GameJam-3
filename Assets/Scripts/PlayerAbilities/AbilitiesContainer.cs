using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInput;
using Core;
using UnityEngine.EventSystems;
using Abilities;

namespace PlayerAbilities
{
    public class AbilitiesContainer : MonoBehaviour
    {
        [SerializeField] private AbilitiesConfiguration _config;
        [SerializeField] private SlimeActior _player;
        [SerializeField] private JumpPointSelector _jumpPointSelector;

        private List<IAbility> _selectedAbilities = new List<IAbility>();

        private int alreadySelectedAbilitiesIndex = -1;
        private float _reloadSpeedMult => _player.ComboCounter.Config.AbilityReloadSpeedMultipler.GetValue(_player.ComboCounter.ComboCount);

        public event Action<IAbility, AbilitySelectPanel.AbilityUIData, int> AbilitySelected;

        public TrySelectAbility SelectAbility { private get; set; }
        public delegate bool TrySelectAbility(AbilitiesConfiguration.AbilitySelection selection, Action<AbilitiesConfiguration.AbilityData> onSelected);

        private InputProvider _inputProvider;

        private void Awake()
        {
            SelectAbility = SelectAbilityPlaceholder;
        }

        private bool SelectAbilityPlaceholder(AbilitiesConfiguration.AbilitySelection selection, Action<AbilitiesConfiguration.AbilityData> onSelected)
        {
            onSelected?.Invoke(UnityEngine.Random.Range(0, 1) > 0.5f ? selection.LeftAbility : selection.RightAbility);
            return true;
        }

        private void Start()
        {
            _inputProvider = ServiceLocator.Get<InputProvider>();
            _inputProvider.AbilityUsed += OnPlayerTryUseAbility;

            for (int i = 0; i < _config.DefaultAbilities.Length; i++)
            {
                var ability = _config.DefaultAbilities[i];
                BuildAndAddAbility(ability);
            }
        }

        private void BuildAndAddAbility(AbilitiesConfiguration.AbilityData data)
        {
            var ability = data.Builder.Build(_player);
            _selectedAbilities.Add(ability);
            AbilitySelected?.Invoke(ability, data.UIData, _selectedAbilities.Count - 1);
        }

        private void OnDestroy()
        {
            _inputProvider.AbilityUsed -= OnPlayerTryUseAbility;
        }

        private void OnPlayerTryUseAbility(int id, InputProvider.IPointerData pointerData)
        {
            if (id >= _selectedAbilities.Count)
                return;

            bool canUse = true;

            var ability = _selectedAbilities[id];

            if(ability is IDirectionalAbility)
            {
                (ability as IDirectionalAbility).Direction = new Vector2(_inputProvider.Horizontal, _inputProvider.Vertical).normalized;
            }

            if(ability is ISlimeAbility)
            {
                (ability as ISlimeAbility).Slime = _player;
            }

            if(ability is IPositionalAbility)
            {
                bool selectingPosition = pointerData.Active;

                _jumpPointSelector.gameObject.SetActive(selectingPosition);

                if (selectingPosition)
                {
                    canUse = false;
                    _jumpPointSelector.SetRadius(_player.Radius);
                    _jumpPointSelector.PositionProvider = new TouchWorldPositionProvider(pointerData, Camera.main).Provide;
                }
                else
                {
                    (ability as IPositionalAbility).WorldPosition = _jumpPointSelector.transform.position;
                    canUse &= _jumpPointSelector.CanJump;
                }

                pointerData.WasUsed = true;
            }

            if(canUse && ability.CanUse())
            {
                ability.Use();
            }
            //_selectedAbilities[id].OnTryUse(data);
        }

        private class TouchWorldPositionProvider
        {
            private InputProvider.IPointerData _data;
            private Camera _camera;

            public TouchWorldPositionProvider(InputProvider.IPointerData data, Camera camera)
            {
                _data = data;
                _camera = camera;
            }

            public Vector2 Provide()
            {
                return _camera.ScreenToWorldPoint(_data.Position);
            }
        }

        private void Update()
        {
            var enoughScoreSelectionIndex = -1;
            var score = _player.CurrentScore;

            for (int i = 0; i < _config.SelectableAbilities.Length; i++)
            {
                if (_config.SelectableAbilities[i].NeededScore <= score)
                    enoughScoreSelectionIndex = i;
                else
                    break;
            }

            if (enoughScoreSelectionIndex > alreadySelectedAbilitiesIndex)
            {
                if (SelectAbility(_config.SelectableAbilities[alreadySelectedAbilitiesIndex + 1], _onAbilitySelected))
                {
                    alreadySelectedAbilitiesIndex++;
                }
            }

            for (int i = 0; i < _selectedAbilities.Count; i++)
            {
                //_selectedAbilities[i].ReloadSpeedMultipler = _reloadSpeedMult;
                _selectedAbilities[i].Update(Time.deltaTime * _reloadSpeedMult);
            }
        }

        private void _onAbilitySelected(AbilitiesConfiguration.AbilityData data)
        {
            BuildAndAddAbility(data);
        }

        public abstract class Ability : MonoBehaviour, IAbility //TODO убрать монобех
        {
            //public AbilitySelectPanel.AbilityUIData UIData; //TODO: удалить
            private protected IActor _actor { get; private set; }

            [SerializeField] private protected float _reloadTime = 0.1f;

            private float _reloadTimer;
            private protected AbilityType AbilityType;

            public event Action OnActivate;
            public event Action OnDeactivate;

            public IAIAbilityInstruction AIAbilityInstruction => null;
            public float Readiness => Mathf.Clamp01(_reloadTimer / _reloadTime);
            public bool Ready => _reloadTimer >= _reloadTime;

            AbilityType IAbility.Type => AbilityType;

            private protected bool CanUse(bool autoReset = true)
            {
                if (Ready && autoReset)
                    ResetTimer();

                return Ready;
            }

            private protected void ResetTimer()
            {
                _reloadTimer = 0;
            }

            public void Init(IActor actor)
            {
                _actor = actor;
                _reloadTimer = _reloadTime; //абилка будет заряжена сразу при получении, можно закомментить тогда не будет
                init();
            }

            public void Update(float dt)
            {
                _reloadTimer += dt;
                update(dt);
            }

            public bool CanUse()
            {
                return Ready && canUse();
            }

            private protected virtual void init() { }

            private protected abstract void update(float dt);

            private protected virtual bool canUse() => true;

            public abstract void Use();
        }
    }
}