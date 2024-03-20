using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilitySelectPart : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _descText;
    [SerializeField] private Image _preview;

    [SerializeField] private Animator _animator;

    private Action _onPress;
    //private bool _pointerOnThis = false;

    public void Init(AbilitySelectPanel.AbilityUIData data, Action onPress)
    {
        _nameText.text = data.Name;
        _descText.text = data.Description;
        _preview.sprite = data.Preview;
        this._onPress = onPress;
    }

    private void OnEnable()
    {
        _animator.SetBool("pointed", false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _animator.SetTrigger("click");
        _onPress();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //_pointerOnThis = true;
        _animator.SetBool("pointed", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _animator.SetBool("pointed", false);
        //_pointerOnThis = false;
    }

    private void Update()
    {

    }
}
