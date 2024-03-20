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

        public override event Action<int, IPointerData> AbilityUsed;
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
            downEntry.callback.AddListener((data) => { AbilityUsed?.Invoke(id, new MobilePointerData((PointerEventData)data)); });

            var upEntry = new EventTrigger.Entry();
            upEntry.eventID = EventTriggerType.PointerUp;
            upEntry.callback.AddListener((data) => { ((PointerEventData)data).Use(); AbilityUsed?.Invoke(id, new MobilePointerData((PointerEventData)data)); });

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

        private class MobilePointerData : IPointerData
        {
            private readonly PointerEventData _pointerEventData;

            public MobilePointerData(PointerEventData data)
            {
                _pointerEventData = data;
            }

            public bool Active => !_pointerEventData.used;

            public Vector2 Position => _pointerEventData.position;

            public bool WasUsed { get; set; }
        }
    }
}