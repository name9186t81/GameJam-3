using PlayerAbilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerInput
{
    public class MobileInputProvider : InputProvider
    {
        [SerializeField] private GameObject _mobileInput;
        [SerializeField] private Joystick _joystick;
        [SerializeField] private AbilitiesUI _mobileAbilitiesUI;

        public override float Horizontal => _joystick.Horizontal;
        public override float Vertical => _joystick.Vertical;

        public override event Action<int> AbilityUsed;

        public override void Init()
        {
            _mobileInput.SetActive(true);
            _mobileAbilitiesUI.Init();
            _mobileAbilitiesUI.OnPartSpawned += OnAbilityPartSpawned;
        }

        private void OnAbilityPartSpawned(AbilityUIPart abilityUIPart, int id)
        {
            abilityUIPart.OnPress += delegate 
            {
                AbilityUsed?.Invoke(id);
            };
        }

        public override void Tick()
        {
            
        }
    }
}