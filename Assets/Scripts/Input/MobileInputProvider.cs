using PlayerAbilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PlayerInput
{
    public class MobileInputProvider : InputProvider
    {
        [SerializeField] private GameObject _mobileInput;
        [SerializeField] private Joystick _joystick;
        [SerializeField] private AbilitiesUI _mobileAbilitiesUI;

        public override float Horizontal => _joystick.Horizontal;
        public override float Vertical => _joystick.Vertical;

        public override event Action<int, PointerEventData> AbilityUsed;
        public override event Action<ActionType> Action;

        public override void Init()
        {
            _mobileInput.SetActive(true);
            _mobileAbilitiesUI.Init();
            _mobileAbilitiesUI.OnPartSpawned += OnAbilityPartSpawned;
        }

        private void OnAbilityPartSpawned(AbilityUIPart abilityUIPart, int id)
        {
            var downEntry = new EventTrigger.Entry();
            downEntry.eventID = EventTriggerType.PointerDown;
            downEntry.callback.AddListener((data) => { AbilityUsed?.Invoke(id, (PointerEventData)data); });

            var upEntry = new EventTrigger.Entry();
            upEntry.eventID = EventTriggerType.PointerUp;
            upEntry.callback.AddListener((data) => { ((PointerEventData)data).Use(); });

            abilityUIPart.Events.triggers.Add(downEntry);
            abilityUIPart.Events.triggers.Add(upEntry);

            abilityUIPart.OnPress += delegate 
            {
                //AbilityUsed?.Invoke(id);
            };
        }

        public void OnPauseButton()
        {
            Action?.Invoke(ActionType.Pause);
        }

        public override void Tick()
        {
            
        }
    }
}