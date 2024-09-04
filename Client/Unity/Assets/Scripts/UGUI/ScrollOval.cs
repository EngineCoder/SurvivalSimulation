using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("UI/Scroll Oval", 38)]//添加到菜单栏面板
[SelectionBase]//将此属性添加到脚本类，以将其GameObject标记为“场景视图”拾取的选择基础对象。
[ExecuteAlways]//始终执行
[DisallowMultipleComponent]//不允许有多个
[RequireComponent(typeof(RectTransform))]//需要这个组件
public class ScrollOval : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public enum MovementType
    {
        Unrestricted,//不受限制的运动
        Elastic,//弹性运动
        Clamped,//夹紧
    }
    
    [SerializeField]
    private RectTransform m_Content;//Content，它应该是带有ScrollOval组件的GameObject的子物体。
    public RectTransform Content { get { return m_Content; } set { m_Content = value; } }

    private RectTransform m_rectTransform;//Content的父物体，设置拖动区域
    public RectTransform RectTransform
    {
        get
        {
            if (m_rectTransform == null)
            {
                m_rectTransform = GetComponent<RectTransform>();
            }
            return m_rectTransform;
        }
    }

    [SerializeField]
    private MovementType m_MovementType = MovementType.Elastic;//内容超出滚动矩形时使用的类型
    public MovementType movementType { get { return m_MovementType; } set { m_MovementType = value; } }

    private bool m_Dragging;//拖动中。。。

    private Vector3 m_PrevRotation = Vector3.zero;

    [SerializeField]
    private float m_Elasticity = 0.1f;//弹性大小
    public float Elasticity { get { return m_Elasticity; } set { m_Elasticity = value; } }


    [SerializeField]
    private float m_DecelerationRate = 0.99f; //减速率仅在启用惯性时使用
    public float DecelerationRate { get { return m_DecelerationRate; } set { m_DecelerationRate = value; } }

    [SerializeField]
    private float m_ScrollSensitivity = 1.0f;//滚动灵敏度
    public float ScrollSensitivity { get { return m_ScrollSensitivity; } set { m_ScrollSensitivity = value; } }

    [System.NonSerialized]
    private Vector3 m_StartContentRotation;
    public Vector3 StartContentRotation
    {
        get { return m_StartContentRotation; }
        set { m_StartContentRotation = value; }
    }

    
    public float internalTime = 0;
    private Vector2 m_StartCursorPointerPosition = Vector2.zero;//开始时鼠标的位置
    public Vector2 StartCursorPointerPosition
    {
        get { return m_StartCursorPointerPosition; }
        set { m_StartCursorPointerPosition = value; }
    }
    
    private Vector2 m_EndCursorPointerPosition = Vector2.zero;//结束时鼠标的位置
    public Vector2 EndCursorPointerPosition
    {
        get { return m_EndCursorPointerPosition; }
        set { m_EndCursorPointerPosition = value; }
    }

    public Vector2 offsetStartEnd = Vector3.zero;

    private Vector2 m_Velocity;//内容的当前速度。
    public Vector2 Velocity { get { return m_Velocity; } set { m_Velocity = value; } }//速度以每秒单位为单位定义。

    private Vector2 m_EndVelocity;//内容的当前速度。
    public Vector2 EndVelocity { get { return m_EndVelocity; } set { m_EndVelocity = value; } }//速度以每秒单位为单位定义。

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        internalTime = 0;
        m_Dragging = true;
        offsetStartEnd = Vector3.zero;

        Vector2 startCursorPosition = Vector2.zero;

        //获得开始拖动时，鼠标在矩形框内的位置
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out startCursorPosition))
            return;
        StartCursorPointerPosition = startCursorPosition;
        StartContentRotation = m_Content.localEulerAngles;//记录当前拖动时的Z轴角度
    }

    /// <summary>
    /// Handling for when the content is dragged.
    /// </summary>
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!m_Dragging)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Vector2 currentCursorPosition = Vector2.zero;

        //如果是在矩形框内滑动的，则 返回当前鼠标的位置
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out currentCursorPosition))
            return;

        Vector2 delta = (currentCursorPosition - StartCursorPointerPosition)*0.4f;

        Vector3 targetRotation = Vector3.zero;

        if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
        {
            targetRotation = StartContentRotation + new Vector3(0, 0, delta.x);
        }
        else
        {
            targetRotation = StartContentRotation + new Vector3(0, 0, delta.y);
        }

        SetContentRotation(targetRotation);
    }

    /// <summary>
    /// Handling for when the content has finished being dragged.
    /// </summary>
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Vector2 endCursorPosition = Vector2.zero;
        //获得开始拖动时，鼠标在矩形框内的位置
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out endCursorPosition))
            return;

        EndCursorPointerPosition = endCursorPosition;

        offsetStartEnd = EndCursorPointerPosition - StartCursorPointerPosition;

        m_Dragging = false;

        EndVelocity = (offsetStartEnd / internalTime)*0.1f;
    }


    void Update()
    {
        if (m_Dragging)
        {
            internalTime += Time.unscaledDeltaTime;
        }
    }

    protected virtual void LateUpdate()
    {
        if (!m_Content)
            return;

        float deltaTime = Time.unscaledDeltaTime;

        float offset = CalculateOffset(EndVelocity);//滑动结束时的速度s

        if (!m_Dragging && EndVelocity != Vector2.zero)
        {
            float speed = 0f;

            Vector3 rotation = m_Content.localEulerAngles;

            rotation[2]=Mathf.SmoothDamp(m_Content.localEulerAngles[2], m_Content.localEulerAngles[2] + offset, ref speed, m_Elasticity, Mathf.Infinity, deltaTime);

            SetContentRotation(rotation);
        }

        if (m_Dragging)
        {
            Vector3 newVelocity = (m_Content.localEulerAngles - m_PrevRotation) / deltaTime;

            m_Velocity = Vector3.Lerp(m_Velocity, newVelocity, deltaTime * 10);
        }

        if (m_Content.localEulerAngles != m_PrevRotation)
        {
            UpdatePrevData();
        }
    }

    protected void UpdatePrevData()
    {
        if (m_Content == null)
            m_PrevRotation = Vector3.zero;
        else
            m_PrevRotation = m_Content.localEulerAngles;
    }


    private float CalculateOffset(Vector2 vector2)
    {
        vector2 *= DecelerationRate;

        if (vector2.magnitude <= 0.01f)
        {
            EndVelocity = Vector2.zero;
        }
        else
        {
            EndVelocity = vector2;
        }
        if (Mathf.Abs(vector2.x) >= Mathf.Abs(vector2.y))
        {
            vector2.y = 0;
            return vector2.x;
        }
        else
        {
            vector2.x = 0;
            return vector2.y;
        }
        
    }



    /// <summary>
    /// Sets the anchored position of the content.
    /// </summary>
    protected virtual void SetContentRotation(Vector3 rotation)
    {
        Content.localEulerAngles = rotation;
    }
}