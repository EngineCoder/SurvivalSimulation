using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Listener_ClickEvent : MonoBehaviour,
    IPointerEnterHandler, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler, IPointerExitHandler
{
   
    Action<GameObject> handler_Click = null;
    Action<GameObject> handler_DoubleClick = null;

    Action<GameObject> handler_PointerDown = null;
    Action<GameObject> handler_PointerUp = null;
    Action<GameObject> handler_PointerEnter = null;
    Action<GameObject> handler_PointerExit = null;


    //移入
    private bool isEnter = false;
    public bool IsEnter
    {
        get
        {
            return isEnter;
        }
    }

 
    public static Listener_ClickEvent Get(GameObject obj)
    {
        Listener_ClickEvent listener_ClickEvent = obj.GetComponent<Listener_ClickEvent>();
        if (listener_ClickEvent == null)
        {
            listener_ClickEvent = obj.AddComponent<Listener_ClickEvent>();
        }
        return listener_ClickEvent;
    }

    /// <summary>
    /// 点击
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {

        if (eventData.clickCount == 2)//Number of clicks in a row.连续点击次数
        {
            Debug.Log(this.gameObject.name + ": 双击");
            handler_DoubleClick?.Invoke(gameObject);//双击
        }
        else
        {
            Debug.LogFormat("{0}" + ": 单击", this.gameObject.name);
            handler_Click?.Invoke(gameObject);
        }

    }

    /// <summary>
    /// 按下
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + ": 按下");
        handler_PointerDown?.Invoke(gameObject);
    }

    /// <summary>
    /// 移入
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + ": 移入");
        isEnter = true;
        handler_PointerEnter?.Invoke(gameObject);
    }

    /// <summary>
    /// 移出
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + ": 移出");
        isEnter = false;
        handler_PointerExit?.Invoke(gameObject);
    }

    /// <summary>
    /// 抬起
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + ": 抬起");
        handler_PointerUp?.Invoke(gameObject);
    }


    public void SetClickHandler(Action<GameObject> handler)
    {
        handler_Click = handler;
    }
    public void SetDoubleClickHandler(Action<GameObject> handler)
    {
        handler_DoubleClick = handler;
    }

    public void SetPointerEnterHandler(Action<GameObject> handler)
    {
        handler_PointerEnter = handler;
    }
    public void SetPointerDownHandler(Action<GameObject> handler)
    {
        handler_PointerDown = handler;
    }
    public void SetPointerUpHandler(Action<GameObject> handler)
    {
        handler_PointerUp = handler;
    }
    public void SetPointerExitHandler(Action<GameObject> handler)
    {
        handler_PointerExit = handler;
    }

}
