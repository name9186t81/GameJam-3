using PlayerAbilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerInput
{
    public class PCInputProvider : InputProvider
    {
        [SerializeField] private Key[] _abilitiesKeys;
        [SerializeField] private AbilitiesUI _pcAbilitiesUI;

        public override float Horizontal => Input.GetAxis("Horizontal");
        public override float Vertical => Input.GetAxis("Vertical");

        public override event Action<int> AbilityUsed;

        [System.Serializable]
        private class Key
        {
            public LocalizationString InterfaceName;
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
            for (int i = 0; i < _abilitiesKeys.Length; i++)
            {
                if (Input.GetKeyDown(_abilitiesKeys[i].KeyCode))
                {
                    AbilityUsed?.Invoke(i);
                }
            }
        }
    }
}