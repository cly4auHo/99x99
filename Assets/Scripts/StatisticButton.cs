using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatisticButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action Hold;
    public Action Release;
    
    public void OnPointerDown(PointerEventData eventData) => Hold?.Invoke();

    public void OnPointerUp(PointerEventData eventData) => Release?.Invoke();
}
