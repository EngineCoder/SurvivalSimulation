using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using STools.Tool_Manager;
public class Effect_UI : Base_Effect
{
    public bool isFirstClick = true;

    public Ease ease = Ease.Unset;

    public Type_EffectPress Type_EffectPress = Type_EffectPress.None;
    public Type_EffectShow Type_EffectShow = Type_EffectShow.None;

    protected override void Start()
    {
        _defaultPos = transform.position;
        _defaultScale = transform.localScale;
        _defaultEuler = transform.eulerAngles;

        if (btn_Self == null)
        {
            btn_Self = transform.GetComponent<Button>();
            if (btn_Self == null)
            {
                tog_Self = transform.GetComponent<Toggle>();
                tog_Self.onValueChanged.AddListener((isOn) =>
                {
                    if (isOn && isFirstClick)
                    {
                        isFirstClick = false;
                        Effect(Type_EffectPress);//点击特效
                    }
                    else if (!isOn && !isFirstClick)
                    {
                        isFirstClick = true;
                        Effect(Type_EffectPress.Press_32);//点击特效
                    }
                });
            }
            else
            {
                btn_Self.onClick.AddListener(() =>
                {
                    Effect(Type_EffectPress);//点击特效
                });
            }
        }

        //如果有展示特效
        if (Type_EffectShow != Type_EffectShow.None)
        {
            Manager_Effect.Instance.GetDelegate_EffectShowMethod((int)Type_EffectShow)?.Invoke(transform);
        }
    }

    protected override void Effect(Type_EffectPress Type_EffectPress)
    {
        if (Type_EffectShow != Type_EffectShow.None)//有展示特效
        {
            if (Type_EffectPress != Type_EffectPress.None)
            {
                ResetDataWhileType_EffectPressNone();
                Manager_Effect.Instance.GetDelegate_EffectPressMethod((int)Type_EffectPress)?.Invoke(transform,ease);
            }
        }
        else//没展示特效有点击特效时
        {
            if (Type_EffectPress != Type_EffectPress.None)
            {
                ResetDataWhileType_EffectShowNone();

                Manager_Effect.Instance.GetDelegate_EffectPressMethod((int)Type_EffectPress)?.Invoke(transform,ease);
            }
        }
    }

    /// <summary>
    /// 重置为按钮默认状态
    /// </summary>
    protected override void ResetDataWhileType_EffectShowNone()
    {
        transform.DOKill();//终止当前特效
        transform.position = _defaultPos;
        transform.localScale = _defaultScale;
        transform.eulerAngles = _defaultEuler;
    }

    /// <summary>
    /// 重置为按钮默认状态
    /// </summary>
    protected override void ResetDataWhileType_EffectPressNone()
    {
        transform.localScale = _defaultScale;
    }
}
