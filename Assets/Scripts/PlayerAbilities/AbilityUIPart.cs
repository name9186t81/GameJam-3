using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        private FloatSmoothDamp _smoothState;
        private AbilitiesContainer.Ability _ability;

        public void Init(AbilitiesContainer.Ability ability)
        {
            _ability = ability;

            _preview.sprite = ability.UIData.Preview;
            _nameText.text = ability.UIData.ReloadingPanelName;

            _smoothState = new FloatSmoothDamp(_smoothStateTime);

            gameObject.SetActive(true);

            Update();
        }

        private void Update()
        {
            var reload = _ability.GetReloadProgress();
            var state = _smoothState.Update(reload >= 1 ? 1 : 0);
            _loadingIndicator.color = new Color(_loadingIndicator.color.r, _loadingIndicator.color.g, _loadingIndicator.color.b, 1 - state);
            _loadingIndicator.fillAmount = reload;
            _preview.transform.localScale = Vector3.one * map01(state, _previewScaleOnReload, 1);
        }

        private float map01(float X, float C, float D)
        {
            return X * (D - C) + C;
        }
    }
}