using UnityEngine;
using UnityEditor;


public class UTools
{
    /// <summary>
    /// 设置锚点的四个钉子为UI矩形的四个
    /// </summary>
    [MenuItem("UTools/Anchors to Corners %[")]
    static void AnchorsToCorners()
    {
        RectTransform t = Selection.activeTransform as RectTransform;
        RectTransform pt = Selection.activeTransform.parent as RectTransform;

        if (t == null || pt == null) return;

        Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
            t.anchorMin.y + t.offsetMin.y / pt.rect.height);
        Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
            t.anchorMax.y + t.offsetMax.y / pt.rect.height);

        t.anchorMin = newAnchorsMin;
        t.anchorMax = newAnchorsMax;
        t.offsetMin = t.offsetMax = new Vector2(0, 0);
    }

    /// <summary>
    /// 设置锚点的位置为Pivot点，仅限在GridLayout规划完成之后
    /// </summary>
    [MenuItem("UTools/Anchors to Pivot %]")]
    static void AnchorsToPivot()
    {
        foreach (GameObject item in Selection.gameObjects)
        {
            //Debug.Log(item.name);
            RectTransform rt = item.transform as RectTransform;
            RectTransform rtParent = item.transform.parent as RectTransform;

            if (rt == null || rtParent == null)
            {
                Debug.LogError("没有选择游戏物体，或者已选择的物体没有父物体");
                return;
            }

            Vector2 newAnchorsMin = new Vector2(rt.anchoredPosition.x / rtParent.rect.width,
                (rt.anchoredPosition.y < 0 ? rtParent.rect.height + rt.anchoredPosition.y : rtParent.rect.height - rt.anchoredPosition.y) / rtParent.rect.height);
            Vector2 newAnchorsMax = new Vector2(rt.anchoredPosition.x / rtParent.rect.width,
                (rt.anchoredPosition.y < 0 ? rtParent.rect.height + rt.anchoredPosition.y : rtParent.rect.height - rt.anchoredPosition.y) / rtParent.rect.height);

            rt.anchorMin = newAnchorsMin;
            rt.anchorMax = newAnchorsMax;
            rt.anchoredPosition = Vector2.zero;
        }
    }

    /// <summary>
    /// 将UI矩形框设置为锚点矩形框
    /// </summary>
    [MenuItem("UTools/Corners to Anchors %|")]
    static void CornersToAnchors()
    {
        RectTransform t = Selection.activeTransform as RectTransform;

        if (t == null) return;
        t.offsetMin = Vector2.zero;
        t.offsetMax = Vector2.zero;
    }
}