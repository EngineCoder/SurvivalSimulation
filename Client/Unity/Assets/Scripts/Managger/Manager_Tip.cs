using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace STools.Tool_Manager
{
    public enum TipKey : byte
    {
        IsNullOrEmpty_Username = 0,
        IsNullOrEmpty_Password,
        IsErrorMobilePhone,
        IsErrorUserNameOrPassword,
        IsErrorUserIdAlreadyRegister,
        IsSuccessfulRegister
    }
    public class Manager_Tip : SingleMode<Manager_Tip>
    {
        public Tip currentTip;

        private Dictionary<int, Tip> tipContent_TipKey_TipContent = new Dictionary<int, Tip>();

        public void Update_TipContent(int tipKey, string tipContent, string title = "温馨提示", string bgImgPath = "", bool isNeedUpdate = false)
        {
            if (isNeedUpdate)
            {
                Tip tip = new Tip(title, tipContent, bgImgPath);

                if (tipContent_TipKey_TipContent.ContainsKey(tipKey))
                {
                    tipContent_TipKey_TipContent[tipKey] = tip;
                }
                else
                {
                    tipContent_TipKey_TipContent.Add(tipKey, tip);
                }

                currentTip = tipContent_TipKey_TipContent[tipKey];
            }
            else
            {
                if (tipContent_TipKey_TipContent.ContainsKey(tipKey))
                {
                    currentTip = tipContent_TipKey_TipContent[tipKey];
                }
                else
                {
                    Tip tip = new Tip(title, tipContent, bgImgPath);
                    tipContent_TipKey_TipContent.Add(tipKey, tip);
                    currentTip = tipContent_TipKey_TipContent[tipKey];
                }
            }
        }

        public class Tip
        {
            public string Title { get; set; }
            public string BGImgPath { get; set; }
            public string TipContent { get; set; }

            public Tip(string title, string tipContent)
            {
                Title = title;
                TipContent = tipContent;
            }

            public Tip(string title, string tipContent, string bgImgPath)
            {
                Title = title;
                TipContent = tipContent;
                BGImgPath = bgImgPath;
            }
        }
    }

}