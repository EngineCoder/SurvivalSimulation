using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using STools.Tool_Manager;
public class Panel_Store : Base_Panel
{
    private Button btn_Close;

    void Start()
    {
        if (btn_Close==null)
        {
            btn_Close = transform.Find("Content_Commodity/Panel_Title/Btn_Close").GetComponent<Button>();
        }

        btn_Close.onClick.AddListener(()=> { Manager_Panel.Instance.PopStack_Panel();});


        canvasGroup = transform.GetComponent<CanvasGroup>();
    }









    public override void OnEnter(float dwellTime, int siblingIndex = 0)//该界面进入时
    {
        this.gameObject.SetActive(true);
    }

    public override void OnExit()//退出该界面，返回到上一界面时，
    {
        this.gameObject.SetActive(false);
    }

    public override void OnPause()//暂停当前界面，跳转到新界面
    {
        canvasGroup.blocksRaycasts = false;
    }

    public override void OnResume()
    {
        canvasGroup.blocksRaycasts = true;
    }
}
