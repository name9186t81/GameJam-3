using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySelectPanel : MonoBehaviour
{
    [SerializeField] private AbilitySelectPart _leftPart;
    [SerializeField] private AbilitySelectPart _rightPart;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _alphaSmoothTime = 0.1f;

    #region test
    [SerializeField] private AbilityUIData _testData;
    [InspectorButton(nameof(testInit))]
    [SerializeField] private bool _testInit;

    private void testInit()
    {
        Init(_testData, _testData, null);
    }
    #endregion

    private float _alphaVel = 0;
    private float _targetAlpha = 0;

    private bool pressed = false;
    private Action<AbilityUIData> _onSelected;

    public void Init(AbilityUIData left, AbilityUIData right, Action<AbilityUIData> onSelected)
    {
        _leftPart.Init(left, OnPress);
        _rightPart.Init(right, OnPress);
        _alphaVel = 0;
        _targetAlpha = 1;
        _canvasGroup.alpha = 0;
        gameObject.SetActive(true);
        pressed = false;
        _onSelected = onSelected;
    }

    private void Update()
    {
        _canvasGroup.alpha = Mathf.SmoothDamp(_canvasGroup.alpha, _targetAlpha, ref _alphaVel, _alphaSmoothTime);
        if(_canvasGroup.alpha < 0.01f && pressed)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnPress(AbilityUIData data)
    {
        _targetAlpha = 0;
        pressed = true;
        _onSelected?.Invoke(data);
    }

    [System.Serializable]
    public struct AbilityUIData
    {
        public string Name;
        [TextArea]
        public string Desc;
        public Sprite Preview;
    }
}
