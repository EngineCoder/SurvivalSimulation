using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Listener_DropEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{

    Action<GameObject> handler_Drop = null;

    public static Listener_DropEvent Get(GameObject obj)
    {
        Listener_DropEvent listener_ClickEvent = obj.GetComponent<Listener_DropEvent>();
        if (listener_ClickEvent == null)
        {
            listener_ClickEvent = obj.AddComponent<Listener_DropEvent>();
        }
        return listener_ClickEvent;
    }


    private bool isEnter = false;
    public bool IsEnter
    {
        get { return isEnter; }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isEnter = true;
    }

    /// <summary>
    /// 放到我这时
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("放下");
        handler_Drop?.Invoke(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isEnter = false;
    }
    public void SetDropHandler(Action<GameObject> handler)
    {
        handler_Drop = handler;
    }
}
