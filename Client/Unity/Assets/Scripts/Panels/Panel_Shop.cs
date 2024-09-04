using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using STools.Tool_Manager;
public class Panel_Shop : Base_Panel
{
    private Button Btn_Close;

    void Awake()
    {
        if (Btn_Close == null) { Btn_Close = transform.Find("Btn_Close").GetComponent<Button>(); }
        Btn_Close.onClick.AddListener(() => { Manager_Panel.Instance.PopStack_Panel(); });
        if (canvasGroup == null)
        {
            canvasGroup.GetComponent<CanvasGroup>();
        }
    }

    public override void OnEnter(float dwellTime=2f, int siblingIndex = 0)//先暂停之前界面，再进入要显示的界面
    {

    }

    public override void OnExit()
    {

    }

    public override void OnPause()
    {

    }

    public override void OnResume()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        //btn_Close.onClick.AddListener(() => { Manager_Panel.Instance.SetPanel(Type_Panel.MainMenuPanel); });
    }
}
