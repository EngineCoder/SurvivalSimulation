using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ParentVChildH : ScrollRect
{
    enum Direction
    {
        Horizontal,
        Vertical
    }

    private Direction m_Direction;
    private Direction m_CurrentDirection;
    private ScrollRect m_ParentScrollRect;
    protected override void Awake()
    {
        base.Awake();

        m_Direction = this.horizontal ? Direction.Horizontal : Direction.Vertical;

        Transform parent = transform.parent.parent;

        if (parent)
            m_ParentScrollRect = parent.GetComponent<ScrollRect>();

    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        Vector2 delta = eventData.delta;

        m_CurrentDirection = Mathf.Abs(delta.x) > Mathf.Abs(delta.y) ? Direction.Horizontal : Direction.Vertical;

        if (m_ParentScrollRect)
        {
            if (m_CurrentDirection != m_Direction)
            {
                ExecuteEvents.Execute(m_ParentScrollRect.gameObject, eventData, ExecuteEvents.beginDragHandler);
                return;
            }
        }
    }


    public override void OnScroll(PointerEventData eventData)
    {
        base.OnScroll(eventData);

        if (m_ParentScrollRect)
        {
            if (m_CurrentDirection != m_Direction)
            {
                ExecuteEvents.Execute(m_ParentScrollRect.gameObject, eventData, ExecuteEvents.scrollHandler);
                return;
            }
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        if (m_ParentScrollRect)
        {
            if (m_CurrentDirection != m_Direction)
            {
                ExecuteEvents.Execute(m_ParentScrollRect.gameObject, eventData, ExecuteEvents.dragHandler);
                return;
            }
        }
    }



    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        if (m_ParentScrollRect)
        {
            if (m_CurrentDirection != m_Direction)
            {
                ExecuteEvents.Execute(m_ParentScrollRect.gameObject, eventData, ExecuteEvents.endDragHandler);
                return;
            }
        }
    }

}
