using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Listener_DragEvent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    Action<GameObject> handler_BeginDrag = null;
    Action<GameObject> handler_Drag = null;
    Action<GameObject> handler_EndDrag = null;

    RectTransform rt;
    Vector3 offset = Vector3.zero;
    Vector3 pos;

    float minWidth;             //水平最小拖拽范围
    float maxWidth;            //水平最大拖拽范围

    float minHeight;            //垂直最小拖拽范围  
    float maxHeight;            //垂直最大拖拽范围

    float rangeX;               //拖拽范围
    float rangeY;               //拖拽范围

    //拖拽
    private bool isDragging = false;
    public bool IsDragging
    {
        get { return isDragging; }
    }

    
    public static Listener_DragEvent Get(GameObject obj)
    {
        Listener_DragEvent listener_ClickEvent = obj.GetComponent<Listener_DragEvent>();
        if (listener_ClickEvent == null)
        {
            listener_ClickEvent = obj.AddComponent<Listener_DragEvent>();
        }
        return listener_ClickEvent;
    }


    void Start()
    {
        rt = GetComponent<RectTransform>();
        pos = rt.position;

        minWidth = rt.rect.width / 2;
        maxWidth = Screen.width - (rt.rect.width / 2);

        minHeight = rt.rect.height / 2;
        maxHeight = Screen.height - (rt.rect.height / 2);
    }


    /// <summary>
    /// 拖拽范围限制
    /// </summary>
    void Update()
    {
        //限制水平/垂直拖拽范围在最小/最大值内
        rangeX = Mathf.Clamp(rt.position.x, minWidth, maxWidth);
        rangeY = Mathf.Clamp(rt.position.y, minHeight, maxHeight);

        //更新位置
        rt.position = new Vector3(rangeX, rangeY, 0);
    }


    /// <summary>
    /// 开始拖动
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + ": 开始拖动");

        //计算UI和鼠标之间的位置偏移量
        offset = rt.position - ScreenPointToWorldPointInRectangle(eventData);
    }

    /// <summary>
    /// 拖动
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + ": 拖动中。。。");
        rt.position = offset + ScreenPointToWorldPointInRectangle(eventData);
    }

    /// <summary>
    /// 拖动结束
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + ": 拖动结束");
    }



    public void SetBeginDragHandler(Action<GameObject> handler)
    {
        handler_BeginDrag = handler;
    }


    public void SetDragHandler(Action<GameObject> handler)
    {
        handler_Drag = handler;
    }
    public void SetEndDragHandler(Action<GameObject> handler)
    {
        handler_EndDrag = handler;
    }



    /// <summary>
    /// Set position of the dragged game object
    /// </summary>
    /// <param name="eventData"></param>
    private Vector3 ScreenPointToWorldPointInRectangle(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        //将屏幕坐标转换成世界坐标
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out globalMousePos);
        return globalMousePos;
    }
}
