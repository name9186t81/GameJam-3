using Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayerAbilities
{
    public class AbilityUIPart : MonoBehaviour
    { 
        [SerializeField] private Image _preview;
        [SerializeField] private Image _loadingIndicator;
        [SerializeField] private Text _nameText;
        [SerializeField] private float _previewScaleOnReload = 0.7f;
        [SerializeField] private float _smoothStateTime = 0.1f;
        public EventTrigger Events;

        private FloatSmoothDamp _smoothState;
        private IAbility _ability;

        public event Action OnPress;

        private string _startName;

        public void Init(IAbility ability, AbilitySelectPanel.AbilityUIData abilityUIData)
        {
            _ability = ability;

            _preview.sprite = abilityUIData.Preview;
            _startName = abilityUIData.Name;

            _smoothState = new FloatSmoothDamp(_smoothStateTime);

            gameObject.SetActive(true);

            OnKeyUpdated("");
            Update();
        }

        public void OnKeyUpdated(string keyDesc)
        {
            _nameText.text = _startName + " " + keyDesc;
        }

        private void Update()
        {
            var reload = _ability.Readiness;
            var state = _smoothState.Update(reload >= 1 ? 1 : 0);
            _loadingIndicator.color = new Color(_loadingIndicator.color.r, _loadingIndicator.color.g, _loadingIndicator.color.b, 1 - state);
            _loadingIndicator.fillAmount = reload;
            _preview.transform.localScale = Vector3.one * state.map01(_previewScaleOnReload, 1);
        }
    }
}