using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 圆形布局组和椭圆布局组的抽象基类，用于通用功能。
/// </summary>
[ExecuteAlways]
public abstract class CircleOrOvalLayoutGroup : MonoBehaviour
{
    protected float x, y, aa, bb;

    private RectTransform m_RectTransform;
    public RectTransform RectTransform
    {
        get
        {
            if (m_RectTransform == null)
                m_RectTransform = GetComponent<RectTransform>();
            return m_RectTransform;
        }
    }
    private float interval_Angular;//角度间隔


    protected void Start()
    {
        aa = (RectTransform.rect.width / 2 * 1f) * (RectTransform.rect.width / 2 * 1f);
        bb = (RectTransform.rect.height / 2 * 1f) * (RectTransform.rect.height / 2 * 1f);
        

        if (RectTransform.childCount == 0)
        {
            return;
        }

        interval_Angular = 360 / RectTransform.childCount;

        for (int i = 0; i < RectTransform.childCount; i++)
        {
            if (interval_Angular * i > 90 && interval_Angular * i < 270)
            {
                x = -Mathf.Sqrt(aa * bb / (bb + aa * Mathf.Tan(interval_Angular * i * Mathf.Deg2Rad) * Mathf.Tan(interval_Angular * i * Mathf.Deg2Rad)));//椭圆标准方程
            }
            else
            {
                x = Mathf.Sqrt(aa * bb / (bb + aa * Mathf.Tan(interval_Angular * i * Mathf.Deg2Rad) * Mathf.Tan(interval_Angular * i * Mathf.Deg2Rad)));//椭圆标准方程
            }

            if (interval_Angular * i == 90)
            {
                y = Mathf.Tan(interval_Angular * i * Mathf.Deg2Rad) * x;
            }
            else
            {
                y = -Mathf.Tan(interval_Angular * i * Mathf.Deg2Rad) * x;
            }

            RectTransform  rectTransform = RectTransform.GetChild(i) as RectTransform;
            rectTransform.anchoredPosition = new Vector2(x, y);
            rectTransform.name = i.ToString();
        }
    }


    //    [SerializeField] protected float m_Spacing = 0;
    //    public float Spacing//设置间距
    //    {
    //        get { return m_Spacing; }
    //        set { SetProperty(ref m_Spacing, value); }
    //    }

    //    //强制扩展子物体的宽度
    //    [SerializeField] protected bool m_ChildForceExpandWidth = true;
    //    public bool ChildForceExpandWidth { get { return m_ChildForceExpandWidth; } set { SetProperty(ref m_ChildForceExpandWidth, value); } }

    //    //强制扩展子物体的高度
    //    [SerializeField] protected bool m_ChildForceExpandHeight = true;
    //    public bool ChildForceExpandHeight { get { return m_ChildForceExpandHeight; } set { SetProperty(ref m_ChildForceExpandHeight, value); } }

    //    //控制子物体的宽度
    //    [SerializeField] protected bool m_ChildControlWidth = true;
    //    public bool ChildControlWidth { get { return m_ChildControlWidth; } set { SetProperty(ref m_ChildControlWidth, value); } }

    //    //控制子物体的高度
    //    [SerializeField] protected bool m_ChildControlHeight = true;
    //    public bool ChildControlHeight { get { return m_ChildControlHeight; } set { SetProperty(ref m_ChildControlHeight, value); } }

    //    //控制子物体的宽度比例
    //    [SerializeField] protected bool m_ChildScaleWidth = false;
    //    public bool ChildScaleWidth { get { return m_ChildScaleWidth; } set { SetProperty(ref m_ChildScaleWidth, value); } }

    //    //控制子物体的高度比例
    //    [SerializeField] protected bool m_ChildScaleHeight = false;
    //    public bool ChildScaleHeight { get { return m_ChildScaleHeight; } set { SetProperty(ref m_ChildScaleHeight, value); } }

    //    //CalcAlongAxis(0 is horizontal and 1 is vertical,isVertical)
    //    protected void CalcAlongAxis(int axis, bool isVertical)
    //    {
    //        float combinedPadding = (axis == 0 ? padding.horizontal : padding.vertical);
    //        bool controlSize = (axis == 0 ? m_ChildControlWidth : m_ChildControlHeight);
    //        bool useScale = (axis == 0 ? m_ChildScaleWidth : m_ChildScaleHeight);
    //        bool childForceExpandSize = (axis == 0 ? m_ChildForceExpandWidth : m_ChildForceExpandHeight);

    //        float totalMin = combinedPadding;
    //        float totalPreferred = combinedPadding;
    //        float totalFlexible = 0;

    //        bool alongOtherAxis = (isVertical ^ (axis == 1));
    //        for (int i = 0; i < rectChildren.Count; i++)
    //        {
    //            RectTransform child = rectChildren[i];
    //            float min, preferred, flexible;
    //            GetChildSizes(child, axis, controlSize, childForceExpandSize, out min, out preferred, out flexible);

    //            if (useScale)
    //            {
    //                float scaleFactor = child.localScale[axis];
    //                min *= scaleFactor;
    //                preferred *= scaleFactor;
    //                flexible *= scaleFactor;
    //            }

    //            if (alongOtherAxis)
    //            {
    //                totalMin = Mathf.Max(min + combinedPadding, totalMin);
    //                totalPreferred = Mathf.Max(preferred + combinedPadding, totalPreferred);
    //                totalFlexible = Mathf.Max(flexible, totalFlexible);
    //            }
    //            else
    //            {
    //                totalMin += min + Spacing;
    //                totalPreferred += preferred + Spacing;

    //                // Increment flexible size with element's flexible size.
    //                totalFlexible += flexible;
    //            }
    //        }

    //        if (!alongOtherAxis && rectChildren.Count > 0)
    //        {
    //            totalMin -= Spacing;
    //            totalPreferred -= Spacing;
    //        }
    //        totalPreferred = Mathf.Max(totalMin, totalPreferred);
    //        SetLayoutInputForAxis(totalMin, totalPreferred, totalFlexible, axis);
    //    }


    //    //设置给定轴的子布局元素的位置和大小。
    //    protected void SetChildrenAlongAxis(int axis, bool isVertical)
    //    {
    //        float size = rectTransform.rect.size[axis];
    //        bool controlSize = (axis == 0 ? m_ChildControlWidth : m_ChildControlHeight);
    //        bool useScale = (axis == 0 ? m_ChildScaleWidth : m_ChildScaleHeight);
    //        bool childForceExpandSize = (axis == 0 ? m_ChildForceExpandWidth : m_ChildForceExpandHeight);
    //        float alignmentOnAxis = GetAlignmentOnAxis(axis);

    //        bool alongOtherAxis = (isVertical ^ (axis == 1));
    //        if (alongOtherAxis)
    //        {
    //            float innerSize = size - (axis == 0 ? padding.horizontal : padding.vertical);
    //            for (int i = 0; i < rectChildren.Count; i++)
    //            {
    //                RectTransform child = rectChildren[i];
    //                float min, preferred, flexible;
    //                GetChildSizes(child, axis, controlSize, childForceExpandSize, out min, out preferred, out flexible);
    //                float scaleFactor = useScale ? child.localScale[axis] : 1f;

    //                float requiredSpace = Mathf.Clamp(innerSize, min, flexible > 0 ? size : preferred);
    //                float startOffset = GetStartOffset(axis, requiredSpace * scaleFactor);
    //                if (controlSize)
    //                {
    //                    SetChildAlongAxisWithScale(child, axis, startOffset, requiredSpace, scaleFactor);
    //                }
    //                else
    //                {
    //                    float offsetInCell = (requiredSpace - child.sizeDelta[axis]) * alignmentOnAxis;
    //                    SetChildAlongAxisWithScale(child, axis, startOffset + offsetInCell, scaleFactor);
    //                }
    //            }
    //        }
    //        else
    //        {
    //            float pos = (axis == 0 ? padding.left : padding.top);
    //            float itemFlexibleMultiplier = 0;
    //            float surplusSpace = size - GetTotalPreferredSize(axis);

    //            if (surplusSpace > 0)
    //            {
    //                if (GetTotalFlexibleSize(axis) == 0)
    //                    pos = GetStartOffset(axis, GetTotalPreferredSize(axis) - (axis == 0 ? padding.horizontal : padding.vertical));
    //                else if (GetTotalFlexibleSize(axis) > 0)
    //                    itemFlexibleMultiplier = surplusSpace / GetTotalFlexibleSize(axis);
    //            }

    //            float minMaxLerp = 0;
    //            if (GetTotalMinSize(axis) != GetTotalPreferredSize(axis))
    //                minMaxLerp = Mathf.Clamp01((size - GetTotalMinSize(axis)) / (GetTotalPreferredSize(axis) - GetTotalMinSize(axis)));

    //            for (int i = 0; i < rectChildren.Count; i++)
    //            {
    //                RectTransform child = rectChildren[i];
    //                float min, preferred, flexible;
    //                GetChildSizes(child, axis, controlSize, childForceExpandSize, out min, out preferred, out flexible);
    //                float scaleFactor = useScale ? child.localScale[axis] : 1f;

    //                float childSize = Mathf.Lerp(min, preferred, minMaxLerp);
    //                childSize += flexible * itemFlexibleMultiplier;
    //                if (controlSize)
    //                {
    //                    SetChildAlongAxisWithScale(child, axis, pos, childSize, scaleFactor);
    //                }
    //                else
    //                {
    //                    float offsetInCell = (childSize - child.sizeDelta[axis]) * alignmentOnAxis;
    //                    SetChildAlongAxisWithScale(child, axis, pos + offsetInCell, scaleFactor);
    //                }
    //                pos += childSize * scaleFactor + Spacing;
    //            }
    //        }
    //    }


    //    //获取子物体的大小
    //    private void GetChildSizes(RectTransform child, int axis, bool controlSize, bool childForceExpand,
    //        out float min, out float preferred, out float flexible)
    //    {
    //        if (!controlSize)
    //        {
    //            min = child.sizeDelta[axis];
    //            preferred = min;
    //            flexible = 0;
    //        }
    //        else
    //        {
    //            min = LayoutUtility.GetMinSize(child, axis);
    //            preferred = LayoutUtility.GetPreferredSize(child, axis);
    //            flexible = LayoutUtility.GetFlexibleSize(child, axis);
    //        }

    //        if (childForceExpand)
    //            flexible = Mathf.Max(flexible, 1);
    //    }




    //#if UNITY_EDITOR
    //    protected override void Reset()
    //    {
    //        base.Reset();
    //        //对于新添加的组件，我们希望将其设置为false，以便用户的尺寸在被覆盖之前不会被覆盖有机会关闭这些设置。
    //        //但是，对于在此之前添加的现有组件，功能已引入，我们希望将其默认设置为向后兼容。
    //        //因此，它们的默认值是on，但是在重置时我们将其设置为off。
    //        m_ChildControlWidth = false;
    //        m_ChildControlHeight = false;
    //    }


    //    private int m_Capacity = 10;
    //    private Vector2[] m_Sizes = new Vector2[10];
    //    protected virtual void Update()
    //    {
    //        if (Application.isPlaying)
    //            return;

    //        int count = transform.childCount;//获得子物体的数量

    //        //如果子物体的数量大于容量
    //        if (count > m_Capacity)
    //        {
    //            if (count > m_Capacity * 2)//如果子物体的数量大于容量的2倍，则扩容到count容量
    //                m_Capacity = count;
    //            else//否则，扩容到m_Capacity容量的2倍
    //                m_Capacity *= 2;

    //            m_Sizes = new Vector2[m_Capacity];
    //        }

    //        //如果子代大小在编辑器中更改，请更新布局（案例945680 - 水平 / 垂直布局组中的子GameObject在编辑器中未显示其正确位置）
    //        // If children size change in editor, update layout (case 945680 - Child GameObjects in a Horizontal/Vertical Layout Group don't display their correct position in the Editor)
    //        bool dirty = false;
    //        for (int i = 0; i < count; i++)
    //        {
    //            //此RectTransform的大小相对于锚点之间的距离。
    //            RectTransform t = transform.GetChild(i) as RectTransform;//获取当前控件
    //            if (t != null && t.sizeDelta != m_Sizes[i])//如果大小改变，则存储当前控件的位置
    //            {
    //                dirty = true;
    //                m_Sizes[i] = t.sizeDelta;
    //            }
    //        }

    //        if (dirty)//将布局标记为要重建
    //            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
    //    }

    //#endif
}

