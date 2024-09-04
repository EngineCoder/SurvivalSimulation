using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using STools.Tool_Manager;
public class Panel_MainMenu : Base_Panel
{
    private Toggle tog_Store;

    void Start()
    {
        if (tog_Store == null)
        {
            //tog_Store = transform.Find("Bottom/Tog_Store").GetComponent<Toggle>();
        }

        //tog_Store.onClick.AddListener(() => { Manager_Panel.Instance.PushStack_Panel(Type_Panel.Store); });

        canvasGroup = transform.GetComponent<CanvasGroup>();
    }




    public override void OnEnter(float dwellTime, int siblingIndex = 0)//该界面进入时
    {
        this.gameObject.SetActive(true);
    }

    public override void OnExit()//退出该界面，返回到上一界面时，
    {

    }

    public override void OnPause()//暂停当前界面，跳转到新界面
    {

    }

    public override void OnResume()
    {

    }
}
