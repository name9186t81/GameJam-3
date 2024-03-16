using PlayerAbilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PlayerInput
{
    public class PCInputProvider : InputProvider
    {
        [SerializeField] private ActionKey[] _actions;
        [SerializeField] private Key[] _abilitiesKeys;
        [SerializeField] private AbilitiesUI _pcAbilitiesUI;

        public override float Horizontal => Input.GetAxis("Horizontal");
        public override float Vertical => Input.GetAxis("Vertical");

        public override event Action<int, PointerEventData> AbilityUsed;
        public override event Action<ActionType> Action;

        [System.Serializable]
        private class Key
        {
            public LocalizationString InterfaceName;
            public KeyCode KeyCode;
        }

        [System.Serializable]
        private class ActionKey
        {
            public ActionType ActionType;
            public KeyCode KeyCode;
        }

        public override void Init()
        {
            _pcAbilitiesUI.Init();
            _pcAbilitiesUI.OnPartSpawned += OnAbilityPartSpawned;
        }

        private void OnAbilityPartSpawned(AbilityUIPart abilityUIPart, int id)
        {
            abilityUIPart.OnKeyUpdated($"[{_abilitiesKeys[id].InterfaceName.Get()}]");
        }

        public override void Tick()
        {
            for (int i = 0; i < _actions.Length; i++)
            {
                if (Input.GetKeyDown(_actions[i].KeyCode))
                {
                    Action?.Invoke(_actions[i].ActionType);
                }
            }

            for (int i = 0; i < _abilitiesKeys.Length; i++)
            {
                if (Input.GetKeyDown(_abilitiesKeys[i].KeyCode))
                {
                    AbilityUsed?.Invoke(i, null);
                }
            }
        }
    }
}