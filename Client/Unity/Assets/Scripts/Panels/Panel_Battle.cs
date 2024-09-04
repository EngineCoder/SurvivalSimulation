using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using STools.Tool_Manager;

public class Panel_Battle : Base_Panel
{
    public Text text_Nickname;
    public Text text_Coin;
    public Text text_Gold;

    public Button btn_Tournament;
    public Button btn_Ranking;

    // Start is called before the first frame update
    void Start()
    {
        text_Nickname.text = Client.user.User_nickname;
        text_Coin.text = Client.user.User_copper_coins.ToString();
        text_Gold.text = Client.user.User_gold_ingot.ToString();

        btn_Tournament.onClick.AddListener(OnClickBtn_Tournament);
        btn_Ranking.onClick.AddListener(OnClickBtn_Ranking);

        canvasGroup = GetComponent<CanvasGroup>();
    }


    private void OnClickBtn_Tournament()
    {
        Manager_Panel.Instance.PushStack_Panel(Type_Panel.Matching);
    }

    private void OnClickBtn_Ranking()
    {
        Manager_Panel.Instance.PushStack_Panel(Type_Panel.Matching);
    }



    public override void OnPause()
    {
        canvasGroup.blocksRaycasts = false;
    }

    public override void OnEnter(float dwellTime,int index)
    {
        this.transform.SetAsFirstSibling();
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
