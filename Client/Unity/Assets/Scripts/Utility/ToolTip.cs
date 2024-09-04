using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ToolTip : MonoBehaviour
{
    private Text toolTipText;
    private Text contentText;
    private CanvasGroup canvasGroup;
    private float targetAlpha = 0;
    public float smoothing = 6;

    /// <summary>
    /// ToolTip位置偏移
    /// </summary>
    private Vector3 toolTipPositionOffset = Vector2.zero;


    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        toolTipText = GetComponent<Text>();
        contentText = transform.Find("Content").GetComponent<Text>();
    }

    void Update()
    {
        if (canvasGroup.alpha != targetAlpha)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, smoothing * Time.deltaTime);
            if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < 0.01f)
            {
                canvasGroup.alpha = targetAlpha;
            }
        }
    }

    /// <summary>
    /// 显示
    /// </summary>
    /// <param name="text"></param>
    public void Show(string text)
    {
        toolTipText.text = text;
        contentText.text = text;
        targetAlpha = 1;
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    public void Hide()
    {
        targetAlpha = 0;
    }

    /// <summary>
    /// 设置提示面板的Pivot和Position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="position"></param>
    public void SetPivotAndLocalPosition(int x, int y, Vector3 position)
    {
        (transform as RectTransform).pivot = new Vector2(x, y);
        transform.localPosition = position;
    }
}
