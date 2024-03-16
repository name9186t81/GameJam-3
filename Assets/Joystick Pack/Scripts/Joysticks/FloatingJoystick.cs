using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    private PointerEventData EventD;

    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        background.gameObject.SetActive(false);
        base.OnPointerUp(EventD);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
        EventD = eventData;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
        EventD = eventData;
    }
}