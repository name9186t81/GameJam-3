using PlayerAbilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySelectPanel : MonoBehaviour
{
    [SerializeField] private InGameUI _inGameUI;
    [SerializeField] private AbilitySelectPart _leftPart;
    [SerializeField] private AbilitySelectPart _rightPart;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _alphaSmoothTime = 0.1f;

    private float _alphaVel = 0;
    private float _targetAlpha = 0;

    private bool pressed = false;

    public bool CanInit => !gameObject.activeSelf;

    private void Awake()
    {
        _inGameUI.AbilitiesContainer.SelectAbility = SelectAbility;
        gameObject.SetActive(false);
    }

    private bool SelectAbility(AbilitiesConfiguration.AbilitySelection selection, Action<AbilitiesConfiguration.AbilityData> onSelected)
    {
        if (!CanInit)
            return false;

        return TryInit(
            selection.LeftAbility.UIData, selection.RightAbility.UIData, 
            delegate {  onSelected?.Invoke(selection.LeftAbility); FinishSelecting(); }, delegate { onSelected?.Invoke(selection.RightAbility); FinishSelecting(); });
    }

    void FinishSelecting()
    {
        _targetAlpha = 0;
        pressed = true;
    }

    public bool TryInit(AbilityUIData left, AbilityUIData right, Action onSelectedLeft, Action onSelectedRight)
    {
        if (gameObject.activeSelf)
            return false;

        _leftPart.Init(left, onSelectedLeft);
        _rightPart.Init(right, onSelectedRight);
        _alphaVel = 0;
        _targetAlpha = 1;
        _canvasGroup.alpha = 0;
        gameObject.SetActive(true);
        pressed = false;

        return true;
    }

    private void Update()
    {
        _canvasGroup.alpha = Mathf.SmoothDamp(_canvasGroup.alpha, _targetAlpha, ref _alphaVel, _alphaSmoothTime);
        if(_canvasGroup.alpha < 0.01f && pressed)
        {
            gameObject.SetActive(false);
        }
    }

    [System.Serializable]
    public struct AbilityUIData
    {
        public LocalizationString Name;
        public LocalizationString Description;
        public Sprite Preview;
    }
}
