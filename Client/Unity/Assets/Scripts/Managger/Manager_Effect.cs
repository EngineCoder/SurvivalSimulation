using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace STools.Tool_Manager
{
    public delegate void Delegate_EffectMethod(Transform transform, Ease ease = Ease.Unset);//
    public class Manager_Effect : SingleMode<Manager_Effect>
    {
        private Dictionary<int, Delegate_EffectMethod> dict_KType_VMethod = new Dictionary<int, Delegate_EffectMethod>();

        public Delegate_EffectMethod GetDelegate_EffectPressMethod(int typeEffect)
        {
            Delegate_EffectMethod delegate_EffectMethod = null;

            if (dict_KType_VMethod.ContainsKey(typeEffect))
            {
                return dict_KType_VMethod[typeEffect];
            }
            else
            {
                switch ((Type_EffectPress)typeEffect)
                {
                    case Type_EffectPress.None:
                        delegate_EffectMethod = None;
                        break;
                    case Type_EffectPress.Press_1:
                        delegate_EffectMethod = Press_1;
                        break;
                    case Type_EffectPress.Press_2:
                        delegate_EffectMethod = Press_2;
                        break;
                    case Type_EffectPress.Press_3:
                        delegate_EffectMethod = Press_3;
                        break;
                    case Type_EffectPress.Press_4:
                        delegate_EffectMethod = Press_4;
                        break;
                    case Type_EffectPress.Press_5:
                        delegate_EffectMethod = Press_5;
                        break;
                    case Type_EffectPress.Press_6:
                        delegate_EffectMethod = Press_6;
                        break;
                    case Type_EffectPress.Press_7:
                        delegate_EffectMethod = Press_7;
                        break;
                    case Type_EffectPress.Press_8:
                        delegate_EffectMethod = Press_8;
                        break;
                    case Type_EffectPress.Press_9:
                        delegate_EffectMethod = Press_9;
                        break;
                    case Type_EffectPress.Press_10:
                        delegate_EffectMethod = Press_10;
                        break;
                    case Type_EffectPress.Press_11:
                        delegate_EffectMethod = Press_11;
                        break;
                    case Type_EffectPress.Press_12:
                        delegate_EffectMethod = Press_12;
                        break;
                    case Type_EffectPress.Press_13:
                        delegate_EffectMethod = Press_13;
                        break;
                    case Type_EffectPress.Press_14:
                        delegate_EffectMethod = Press_14;
                        break;
                    case Type_EffectPress.Press_15:
                        delegate_EffectMethod = Press_15;
                        break;
                    case Type_EffectPress.Press_16:
                        delegate_EffectMethod = Press_16;
                        break;
                    case Type_EffectPress.Press_17:
                        delegate_EffectMethod = Press_17;
                        break;
                    case Type_EffectPress.Press_18:
                        delegate_EffectMethod = Press_18;
                        break;
                    case Type_EffectPress.Press_19:
                        delegate_EffectMethod = Press_19;
                        break;
                    case Type_EffectPress.Press_20:
                        delegate_EffectMethod = Press_20;
                        break;
                    case Type_EffectPress.Press_21:
                        delegate_EffectMethod = Press_21;
                        break;
                    case Type_EffectPress.Press_22:
                        delegate_EffectMethod = Press_22;
                        break;
                    case Type_EffectPress.Press_23:
                        delegate_EffectMethod = Press_23;
                        break;
                    case Type_EffectPress.Press_24:
                        delegate_EffectMethod = Press_24;
                        break;
                    case Type_EffectPress.Press_25:
                        delegate_EffectMethod = Press_25;
                        break;
                    case Type_EffectPress.Press_26:
                        delegate_EffectMethod = Press_26;
                        break;
                    case Type_EffectPress.Press_27:
                        delegate_EffectMethod = Press_27;
                        break;
                    case Type_EffectPress.Press_28:
                        delegate_EffectMethod = Press_28;
                        break;
                    case Type_EffectPress.Press_29:
                        delegate_EffectMethod = Press_29;
                        break;
                    case Type_EffectPress.Press_30:
                        delegate_EffectMethod = Press_30;
                        break;
                    case Type_EffectPress.Press_31:
                        delegate_EffectMethod = Press_31;
                        break;
                    case Type_EffectPress.Press_32:
                        delegate_EffectMethod = Press_32;
                        break;
                    case Type_EffectPress.Press_33:
                        delegate_EffectMethod = Press_33;
                        break;
                    case Type_EffectPress.Press_34:
                        delegate_EffectMethod = Press_34;
                        break;
                    default:
                        break;
                }
                if (delegate_EffectMethod != null)
                {
                    dict_KType_VMethod.Add(typeEffect, delegate_EffectMethod);
                }
                return delegate_EffectMethod;
            }
        }

        public Delegate_EffectMethod GetDelegate_EffectShowMethod(int typeEffect)
        {
            Delegate_EffectMethod delegate_EffectMethod = null;
            if (dict_KType_VMethod.ContainsKey(typeEffect))
            {
                return dict_KType_VMethod[typeEffect];
            }
            else
            {
                switch ((Type_EffectShow)typeEffect)
                {
                    case Type_EffectShow.None:
                        delegate_EffectMethod = None;
                        break;
                    case Type_EffectShow.Show_1:
                        delegate_EffectMethod = Show_1;
                        break;
                    case Type_EffectShow.Show_2:
                        delegate_EffectMethod = Show_2;
                        break;
                    case Type_EffectShow.Show_3:
                        delegate_EffectMethod = Show_3;
                        break;
                    case Type_EffectShow.Show_4:
                        delegate_EffectMethod = Show_4;
                        break;
                    case Type_EffectShow.Show_5:
                        delegate_EffectMethod = Show_5;
                        break;
                    case Type_EffectShow.Show_6:
                        delegate_EffectMethod = Show_6;
                        break;
                    case Type_EffectShow.Show_7:
                        delegate_EffectMethod = Show_7;
                        break;
                    case Type_EffectShow.Show_8:
                        delegate_EffectMethod = Show_8;
                        break;
                    case Type_EffectShow.Show_9:
                        delegate_EffectMethod = Show_9;
                        break;
                    case Type_EffectShow.Show_10:
                        delegate_EffectMethod = Show_10;
                        break;
                    case Type_EffectShow.Show_11:
                        delegate_EffectMethod = Show_11;
                        break;
                    case Type_EffectShow.Show_12:
                        delegate_EffectMethod = Show_12;
                        break;
                    case Type_EffectShow.Show_13:
                        delegate_EffectMethod = Show_13;
                        break;
                    default:
                        break;
                }
                if (delegate_EffectMethod != null)
                {
                    dict_KType_VMethod.Add(typeEffect, delegate_EffectMethod);
                }
                return delegate_EffectMethod;
            }
        }


        #region EffectMethod_Press
        public void None(Transform transform, Ease ease)
        {

        }

        public void Press_1(Transform transform, Ease ease)
        {
            transform.DOPunchScale(new Vector3(-0.2f, 0, 0), 0.4f, 12, 1);
        }

        public void Press_2(Transform transform, Ease ease)
        {
            transform.DOPunchScale(new Vector3(0, -0.2f, 0), 0.4f, 12, 0.5f);
        }

        public void Press_3(Transform transform, Ease ease)
        {
            transform.DOPunchScale(new Vector3(-0.2f, -0.2f, 0), 0.4f, 12, 0.5f);
        }

        public void Press_4(Transform transform, Ease ease)
        {
            transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.4f, 12, 0.5f);
        }

        public void Press_5(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(0, 20, 0), 0.2f, 4, 0.5f);//.GetComponent<RectTransform>()
        }

        public void Press_6(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(0, 40, 0), 0.4f, 12, 0.5f);
            transform.DOPunchRotation(new Vector3(0, 0, 30), 0.4f, 12, 0.5f);
        }

        public void Press_7(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(0, 40, 0), 0.4f, 12, 0.5f);
            transform.DOPunchRotation(new Vector3(0, 0, -30), 0.4f, 12, 0.5f);
        }

        public void Press_8(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(0, 20, 0), 0.2f, 4, 0.5f);
            transform.DOPunchRotation(new Vector3(0, 0, 30), 0.4f, 4, 0.5f);
        }

        public void Press_9(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(0, 20, 0), 0.2f, 4, 0.5f);
            transform.DOPunchRotation(new Vector3(0, 0, -30), 0.4f, 4, 0.5f);
        }

        public void Press_10(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(0, 20, 0), 0.2f, 4, 0.5f);
        }

        public void Press_11(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(0, 40, 0), 0.4f, 4, 0.5f);
            transform.DOPunchScale(new Vector3(0.4f, 0, 0), 0.3f, 4, 0.5f).SetDelay(0.1f);
        }

        public void Press_12(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(0, 40, 0), 0.4f, 4, 0.5f);
            transform.DOPunchScale(new Vector3(0, 0.2f, 0), 0.3f, 4, 0.5f).SetDelay(0.1f);
        }

        public void Press_13(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(0, 40, 0), 0.4f, 8, 0.5f);
            transform.DOPunchScale(new Vector3(0.1f, -0.2f, 0), 0.4f, 12, 0.5f);
        }

        public void Press_14(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(0, 20, 0), 0.2f, 4, 0.5f);
            transform.DOPunchScale(new Vector3(0.1f, 0.2f, 0), 0.4f, 12, 0.5f);
        }

        public void Press_15(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(-80, 0, 0), 0.4f, 4, 1f);
            transform.DOPunchRotation(new Vector3(0, 0, 30), 0.3f, 4, 0.5f).SetDelay(0.1f);
            transform.DOPunchScale(new Vector3(-0.2f, 0.2f, 0), 0.4f, 16, 0.5f);
        }

        public void Press_16(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(-40, 0, 0), 0.4f, 4, 1f);
            transform.DOPunchScale(new Vector3(-0.2f, 0.2f, 0), 0.4f, 16, 0.5f);
        }

        public void Press_17(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(40, 0, 0), 0.4f, 4, 1f);
            transform.DOPunchScale(new Vector3(-0.2f, 0.2f, 0), 0.4f, 16, 0.5f);
        }

        public void Press_18(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(80, 0, 0), 0.4f, 4, 1f);
            transform.DOPunchRotation(new Vector3(0, 0, -30), 0.3f, 4, 0.5f).SetDelay(0.1f);
            transform.DOPunchScale(new Vector3(-0.2f, 0.2f, 0), 0.4f, 16, 0.5f);
        }

        public void Press_19(Transform transform, Ease ease)
        {
            transform.DOPunchRotation(new Vector3(0, 0, 30), 0.2f, 4, 0.5f);
        }

        public void Press_20(Transform transform, Ease ease)
        {
            transform.DOPunchRotation(new Vector3(0, 0, -30), 0.2f, 4, 0.5f);
        }

        public void Press_21(Transform transform, Ease ease)
        {
            transform.DOPunchScale(new Vector3(-0.2f, -0.2f, 0), 0.2f, 4, 0.5f);
        }

        public void Press_22(Transform transform, Ease ease)
        {
            transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.2f, 4, 0.5f);
        }

        public void Press_23(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(-20, 0, 0), 0.2f, 4, 0.5f);
            transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.2f, 4, 0.5f);
        }

        public void Press_24(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(0, 20, 0), 0.2f, 4, 0.5f);
            transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.2f, 4, 0.5f);
        }

        public void Press_25(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(0, -20, 0), 0.2f, 4, 0.5f);
            transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.2f, 4, 0.5f);
        }

        public void Press_26(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(20, 0, 0), 0.2f, 4, 0.5f);
            transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.2f, 4, 0.5f);
        }

        public void Press_27(Transform transform, Ease ease)
        {
            transform.DOPunchRotation(new Vector3(0, 0, 30), 0.2f, 4, 0.5f);
            transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.2f, 4, 0.5f);
        }

        public void Press_28(Transform transform, Ease ease)
        {
            transform.DOPunchRotation(new Vector3(0, 0, -30), 0.2f, 4, 0.5f);
            transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.2f, 4, 0.5f);
        }

        public void Press_29(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(-40, 0, 0), 0.4f, 4, 0.5f);
            transform.DOPunchRotation(new Vector3(0, 0, 30), 0.4f, 4, 0.5f);
        }

        public void Press_30(Transform transform, Ease ease)
        {
            transform.DOPunchPosition(new Vector3(40, 0, 0), 0.4f, 4, 0.5f);
            transform.DOPunchRotation(new Vector3(0, 0, -30), 0.4f, 4, 0.5f);
        }


        public void Press_31(Transform transform, Ease ease)
        {
            RectTransform rect = (transform as RectTransform);
            Tween tween = rect.DOAnchorPos3DY(8, 0.1f);
            tween.SetEase(ease);
        }


        public void Press_32(Transform transform, Ease ease)
        {
            RectTransform rect = (transform as RectTransform);
            Tween tween = rect.DOAnchorPos3DY(0, 0.1f);
            tween.SetEase(ease);
        }



        public void Press_33(Transform transform, Ease ease)
        {
            Tween t = (transform as RectTransform).DOAnchorPos3DY(140, 0.5f);
            t.SetEase(ease);
            t.SetLoops(-1);
        }

        public void Press_34(Transform transform, Ease ease)
        {
            Tween t = (transform as RectTransform).DOAnchorPos3DY(-140, 2);
            t.SetEase(ease);
            t.SetLoops(-1);
        }


        #endregion

        #region EffectMethod_Show

        public void Show_1(Transform transform, Ease ease)
        {
            transform.DOMove(transform.position + new Vector3(0, 10, 0), 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void Show_2(Transform transsform, Ease ease)
        {
            Image image = transform.GetComponent<Image>();
            image.CrossFadeAlpha(0.4f, 0.5f, true);
        }

        public void Show_3(Transform transform, Ease ease)
        {
            transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            transform.DOScale(new Vector3(1, 1, 1), 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);

        }

        public void Show_4(Transform transform, Ease ease)
        {
            transform.DOMove(transform.position + new Vector3(5, 0, 0), 0.5f)
                   .SetEase(Ease.InOutCubic)
                   .SetLoops(-1, LoopType.Yoyo);
            transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            transform.DOScale(new Vector3(1, 1, 1), 0.25f)
                .SetEase(Ease.InOutCubic)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void Show_5(Transform transform, Ease ease)
        {
            transform.DOMove(transform.position + new Vector3(0, 5, 0), 0.5f)
                 .SetEase(Ease.InOutCubic)
                 .SetLoops(-1, LoopType.Yoyo);
            transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            transform.DOScale(new Vector3(1, 1, 1), 0.25f)
                .SetEase(Ease.InOutCubic)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void Show_6(Transform transform, Ease ease)
        {
            transform.DOMove(transform.position + new Vector3(10, 0, 0), 1f)
                  .SetEase(Ease.InOutSine)
                  .SetLoops(-1, LoopType.Yoyo);
        }

        public void Show_7(Transform transform, Ease ease)
        {
            transform.DOMove(transform.position + new Vector3(0, 10, 0), 1f)
               .SetEase(Ease.InOutElastic)
               .SetLoops(-1, LoopType.Yoyo);
        }

        public void Show_8(Transform transform, Ease ease)
        {
            transform.DOMove(transform.position + new Vector3(10, 0, 0), 1f)
                .SetEase(Ease.InOutElastic)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void Show_9(Transform transform, Ease ease)
        {
            transform.eulerAngles = new Vector3(0, 0, -10);
            transform.DORotate(new Vector3(0, 0, 10), 1f)
           .SetEase(Ease.InOutElastic)
           .SetLoops(-1, LoopType.Yoyo);
        }

        public void Show_10(Transform transform, Ease ease)
        {
            transform.eulerAngles = new Vector3(0, 0, -10);
            transform.DORotate(new Vector3(0, 0, 10), 0.5f)
           .SetEase(Ease.InOutSine)
           .SetLoops(-1, LoopType.Yoyo);
        }

        public void Show_11(Transform transform, Ease ease)
        {
            transform.DOMove(transform.position + new Vector3(10, 0, 0), 0.2f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void Show_12(Transform transform, Ease ease)
        {
            transform.DOMove(transform.position + new Vector3(0, 10, 0), 0.2f)
             .SetEase(Ease.InOutSine)
             .SetLoops(-1, LoopType.Yoyo);
        }

        public void Show_13(Transform transform, Ease ease)
        {
            transform.DOPunchRotation(new Vector3(0, 0, -41), 4f, 0, 0)
                .SetEase(Ease.Linear)
                .SetLoops(-1);
        }

        #endregion
    }
}