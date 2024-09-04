using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using STools.Tool_Manager;
public class Panel_Matching : Base_Panel
{
    public DOTweenPath tweenPath;

    public Button btn_Return;
    public Text text;
    private float timer;
    private float intervalTime=0.5f;
    private string tempStr=string.Empty;
    private int count = 0;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        tweenPath.DORestart(true);
    }
    void Start()
    {
        

        btn_Return.onClick.AddListener(OnClickBtn_Return);
        tempStr = "搜索对手";

    }
    void Update()
    {



        timer += Time.unscaledDeltaTime;
        if (timer>=intervalTime)
        {
            count++;
            timer = 0;
            if (count > 3)
            {
                tempStr = "搜索对手";
                count = 0;
            }
            else
            {
                tempStr += "。";
            }

            text.text = tempStr;
        }
    }
    private void OnClickBtn_Return()
    {
        //面板隐藏
        Manager_Panel.Instance.PopStack_Panel();
    }



    public override void OnPause()
    {
        canvasGroup.blocksRaycasts = false;
    }

    public override void OnEnter(float dwellTime,int siblingIndex)
    {
        this.gameObject.SetActive(true);
    }

    public override void OnResume()
    {
        canvasGroup.blocksRaycasts = true;
    }

    public override void OnExit()
    {
        this.gameObject.SetActive(false);
    }
}
