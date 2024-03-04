using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySelectPanel : MonoBehaviour
{
    [SerializeField] private AbilitySelectPart _leftPart;
    [SerializeField] private AbilitySelectPart _rightPart;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _alphaSmoothTime = 0.1f;

    [SerializeField] private AbilityUIData _testData;

    private float _alphaVel = 0;
    private float _targetAlpha = 0;

    private void Start()
    {
        //test
        Init(_testData, _testData);
    }

    public void Init(AbilityUIData left, AbilityUIData right)
    {
        _leftPart.Init(left, OnPress);
        _rightPart.Init(right, OnPress);
        _alphaVel = 0;
        _targetAlpha = 1;
        _canvasGroup.alpha = 0;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        _canvasGroup.alpha = Mathf.SmoothDamp(_canvasGroup.alpha, _targetAlpha, ref _alphaVel, _alphaSmoothTime);
        if(_canvasGroup.alpha < 0.01f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnPress(AbilityUIData data)
    {
        _targetAlpha = 0;
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
