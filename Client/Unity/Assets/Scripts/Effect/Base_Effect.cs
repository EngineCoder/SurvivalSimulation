using UnityEngine;
using UnityEngine.UI;

public enum Type_EffectShow : short
{
    None = 0,
    Show_1,
    Show_2,
    Show_3,
    Show_4,
    Show_5,
    Show_6,
    Show_7,
    Show_8,
    Show_9,
    Show_10,
    Show_11,
    Show_12,
    Show_13
}
public enum Type_EffectPress : short
{
    None = 1001,
    Press_1,
    Press_2,
    Press_3,
    Press_4,
    Press_5,
    Press_6,
    Press_7,
    Press_8,
    Press_9,
    Press_10,
    Press_11,
    Press_12,
    Press_13,
    Press_14,
    Press_15,
    Press_16,
    Press_17,
    Press_18,
    Press_19,
    Press_20,
    Press_21,
    Press_22,
    Press_23,
    Press_24,
    Press_25,
    Press_26,
    Press_27,
    Press_28,
    Press_29,
    Press_30,
    Press_31,
    Press_32,
    Press_33,
    Press_34
}

public abstract class Base_Effect : MonoBehaviour
{
    protected Button btn_Self;
    protected Toggle tog_Self;

    protected Vector3 _defaultPos;
    protected Vector3 _defaultScale;
    protected Vector3 _defaultEuler;

    /// <summary>
    /// 初始化
    /// </summary>
    protected abstract void Start();

    /// <summary>
    /// 展示特效
    /// </summary>
    protected abstract void Effect(Type_EffectPress type_EffectPress);

    /// <summary>
    /// 重置复位
    /// </summary>
    protected abstract void ResetDataWhileType_EffectPressNone();

    /// <summary>
    /// 重置复位
    /// </summary>
    protected abstract void ResetDataWhileType_EffectShowNone();

}
