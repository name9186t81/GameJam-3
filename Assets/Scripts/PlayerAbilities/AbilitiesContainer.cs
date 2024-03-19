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
        [SerializeField] private AbilitySelection[] _abilities;
        [SerializeField] private PlayerActor _player;
        [SerializeField] private AbilitySelectPanel _selectPanel;

        [SerializeField] private float _reloadSpeedBonusPerCombo = 0.02f;
        [SerializeField][Min(1)] private float _reloadSpeedPerComboLimit = 1.5f;

        [SerializeField] private List<Ability> _selectedAbilities = new List<Ability>();

        private int alreadySelectedAbilitiesIndex = -1;
        private int _selectionsPendingCount = 0; //чтобы не показывать следующий выбор способности пока предыдущий еще не выбран
        private float _reloadSpeedMult => Mathf.Min(1 + ComboUI.ComboCount * _reloadSpeedBonusPerCombo, _reloadSpeedPerComboLimit);

        public event Action<Ability, int> OnAbilitySelected;

        private InputProvider _inputProvider;

        private void Start()
        {
            _inputProvider = ServiceLocator.Get<InputProvider>();
            _inputProvider.AbilityUsed += OnTryUseAbility;

            for (int i = 0; i < _selectedAbilities.Count; i++)
            {
                _selectedAbilities[i]._usingMobileInput = _inputProvider.UsingMobileInput;
                _selectedAbilities[i].Init(_player);
                OnAbilitySelected?.Invoke(_selectedAbilities[i], i);
            }
        }

        private void OnDestroy()
        {
            _inputProvider.AbilityUsed -= OnTryUseAbility;
        }

        private void OnTryUseAbility(int id, PointerEventData data)
        {
            if (id >= _selectedAbilities.Count)
                return;

            _selectedAbilities[id].OnTryUse(data);
        }

        private void Update()
        {
            var enoughScoreSelectionIndex = -1;
            var score = _player.CurrentScore;

            for (int i = 0; i < _abilities.Length; i++)
            {
                if (_abilities[i].NeededScore <= score)
                    enoughScoreSelectionIndex = i;
                else
                    break;
            }

            if (enoughScoreSelectionIndex > alreadySelectedAbilitiesIndex && _selectionsPendingCount == 0 && _selectPanel.CanInit)
            {
                alreadySelectedAbilitiesIndex++;
                _abilities[alreadySelectedAbilitiesIndex].Show(_selectPanel, _onAbilitySelected, _player, _inputProvider);
                _selectionsPendingCount++;
            }

            for (int i = 0; i < _selectedAbilities.Count; i++)
            {
                _selectedAbilities[i].ReloadSpeedMultipler = _reloadSpeedMult;
                _selectedAbilities[i].Update(Time.deltaTime);
            }
        }

        private void _onAbilitySelected(Ability ability)
        {
            _selectedAbilities.Add(ability);
            _selectionsPendingCount--;
            OnAbilitySelected?.Invoke(ability, _selectedAbilities.Count - 1);
        }

        [System.Serializable]
        public class AbilitySelection
        {
            public float NeededScore;
            public Ability LeftAbility;
            public Ability RightAbility;

            public void Show(AbilitySelectPanel panel, Action<Ability> OnAbilitySelected, PlayerActor player, InputProvider provider)
            {
                panel.TryInit(LeftAbility.UIData, RightAbility.UIData, delegate (AbilitySelectPanel.AbilityUIData data)
                {
                    //да, говно, но лучше не придумал
                    if (data.Equals(LeftAbility.UIData))
                    {
                        LeftAbility._usingMobileInput = provider.UsingMobileInput;
                        LeftAbility.Init(player);
                        OnAbilitySelected?.Invoke(LeftAbility);
                    }
                    else if (data.Equals(RightAbility.UIData))
                    {
                        RightAbility._usingMobileInput = provider.UsingMobileInput;
                        RightAbility.Init(player);
                        OnAbilitySelected?.Invoke(RightAbility);
                    }
                });
            }
        }

        public abstract class Ability : MonoBehaviour, IAbility
        {
            [SerializeField] private AbilityType AbilityType;
            public AbilitySelectPanel.AbilityUIData UIData;
            private protected IActor _actor { get; private set; }
            private protected PointerEventData _lastPointerEventData { get; private set; } = null;
            private protected Vector2 _cursorWorldPos => Camera.main.ScreenToWorldPoint(Input.mousePosition);

            [SerializeField] private float _reloadTime = 0.1f;
            private float _timerReload;

            private float _lastTryUseTime = -1000;
            private const float _useThresholdSeconds = 0.1f;

            public event Action OnActivate;
            public event Action OnDeactivate;

            [HideInInspector] public float ReloadSpeedMultipler = 1;
            [HideInInspector] public bool _usingMobileInput = false; //TODO: удалить

            public IAIAbilityInstruction AIAbilityInstruction => null;
            public float Readiness => Mathf.Clamp01(_timerReload / _reloadTime);

            AbilityType IAbility.Type => AbilityType;

            public void OnTryUse(PointerEventData data)
            {
                _lastPointerEventData = data;
                _lastTryUseTime = Time.unscaledTime;
            }

            private protected bool CanUse(bool autoReset = true)
            {
                if (Time.unscaledTime - _lastTryUseTime > _useThresholdSeconds)
                    return false;

                bool result = _timerReload > _reloadTime;
                if (result && autoReset)
                    ResetTimer();

                return result;
            }

            private protected void ResetTimer()
            {
                _timerReload = 0;
            }

            public void Init(IActor actor)
            {
                _actor = actor;
                _timerReload = _reloadTime; //абилка будет заряжена сразу при получении, можно закомментить тогда не будет
            }

            public void Update(float dt)
            {
                _timerReload += dt * ReloadSpeedMultipler;
                update();
            }

            private protected abstract void update();

            public bool CanUse()
            {
                return CanUse(false);
            }

            public abstract void Use();
        }
    }
}