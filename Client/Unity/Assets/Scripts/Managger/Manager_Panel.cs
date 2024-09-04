using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace STools.Tool_Manager
{
    public enum Type_Panel
    {
        MainMenu,
        Knapsack,
        Setting,
        Shop,
        Tip,
        Register,
        Login,
        Store,
        Battle,
        Matching
    }


    public class Manager_Panel/* : SingleMode<Manager_Panel>*/
    {
        private static Manager_Panel _instance;
        public static Manager_Panel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Manager_Panel();
                }
                return _instance;
            }
        }

        private Transform transform_Canvas;
        public Transform Transform_Canvas
        {
            get
            {
                if (transform_Canvas == null)
                {
                    transform_Canvas = GameObject.Find("Canvas").transform;
                }
                return transform_Canvas;
            }
        }

        private Dictionary<Type_Panel, string> dict_TypePanel_Path = new Dictionary<Type_Panel, string>();
        private Dictionary<Type_Panel, Base_Panel> dict_TypePanel_BasePanel = new Dictionary<Type_Panel, Base_Panel>();

        /// <summary>
        /// 界面存储栈
        /// </summary>
        private Stack<Base_Panel> stack_Panel = new Stack<Base_Panel>();

        #region Initialize
        public void Initialize()
        {

        }
        public Manager_Panel()
        {
            ParseJson_PanelInfo();
        }
        #endregion

        /// <summary>
        /// 解析面板信息
        /// </summary>
        private void ParseJson_PanelInfo()
        {
            TextAsset textAssetPanelInfo = Resources.Load<TextAsset>("TextAsset/Json/PanelInfo");
            UIPanelInfo uIPanelInfo = JsonUtility.FromJson<UIPanelInfo>(textAssetPanelInfo.text);

            foreach (PanelInfo item in uIPanelInfo.panelInfoList)
            {
                Type_Panel type_Panel = (Type_Panel)Enum.Parse(typeof(Type_Panel), item.panelType);
                dict_TypePanel_Path.Add(type_Panel, item.path);
            }
        }

        /// <summary>
        /// 获得面板
        /// </summary>
        /// <param name="type_Panel"></param>
        /// <returns></returns>
        private Base_Panel GetPanel(Type_Panel type_Panel)
        {
            Base_Panel base_Panel = dict_TypePanel_BasePanel.GetValue(type_Panel);

            if (base_Panel == null)
            {
                string path = dict_TypePanel_Path.GetValue(type_Panel);
                Transform panelTransform = GameObject.Instantiate(Resources.Load<GameObject>(path), Transform_Canvas).transform;//ToolTip
                base_Panel = panelTransform.GetComponent<Base_Panel>();
                dict_TypePanel_BasePanel.Add(type_Panel, base_Panel);
            }
            return base_Panel;
        }


        /// <summary>
        /// 入栈，显示面板
        /// </summary>
        /// <param name="type_Panel"></param>
        public void PushStack_Panel(Type_Panel type_Panel, float dwellTime = 0.5f, int siblingIndex = 0)
        {
            if (stack_Panel == null)
            {
                stack_Panel = new Stack<Base_Panel>();
            }

            //停止上一个界面
            if (stack_Panel.Count > 0)
            {
                Base_Panel topOldPanel = stack_Panel.Peek();
                topOldPanel.OnPause();
            }

            //显示新界面
            Base_Panel topNewPanel = GetPanel(type_Panel);
            stack_Panel.Push(topNewPanel);
            topNewPanel.OnEnter(dwellTime, siblingIndex);
        }


        /// <summary>
        /// 出栈，隐藏面板
        /// </summary>
        public void PopStack_Panel()
        {
            if (stack_Panel == null)
            {
                stack_Panel = new Stack<Base_Panel>();
            }
            if (stack_Panel.Count <= 0)
            {
                return;
            }

            //栈顶面板退出
            Base_Panel topOldPanel = stack_Panel.Pop();
            topOldPanel.OnExit();

            //恢复上一个面板
            if (stack_Panel.Count > 0)
            {
                Base_Panel topNewPanel = stack_Panel.Peek();
                topNewPanel.OnResume();
            }
        }

        /// <summary>
        /// 清空栈
        /// </summary>
        public void ClearAllStackElement()
        {
            stack_Panel.Clear();
        }
    }


    public static class DictTool
    {
        public static Tvalue GetValue<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict, Tkey key)
        {
            Tvalue value = default(Tvalue);
            dict.TryGetValue(key, out value);
            return value;
        }
    }


    [System.Serializable]
    public class UIPanelInfo
    {
        public List<PanelInfo> panelInfoList;
    }


    [System.Serializable]
    public class PanelInfo
    {
        public string panelType;
        public string path;
    }


    public abstract class Base_Panel : MonoBehaviour
    {
        protected CanvasGroup canvasGroup;

        //暂停
        public abstract void OnPause();

        //进入
        public abstract void OnEnter(float dwellTime, int siblingIndex);//先暂停上一个界面，再进入

        //恢复
        public abstract void OnResume();//恢复上一个界面

        //退出
        public abstract void OnExit();//退出当前界面，恢复上一个界面
    }
}