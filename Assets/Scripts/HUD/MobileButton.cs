using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action<bool> onPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        onPressed?.Invoke(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPressed?.Invoke(false);
    }
}
