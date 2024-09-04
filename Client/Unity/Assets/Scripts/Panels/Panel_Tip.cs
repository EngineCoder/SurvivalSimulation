using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Networking;
using STools.Tool_Manager;
public class Panel_Tip : Base_Panel
{
    private Image img_BG;

    private Text text_Panel_ToolTip;
    private Text text_Title;
    private Text text_Content;


    void Awake()
    {
        //Image
        if (img_BG == null)
        {
            img_BG = transform.Find("Img_BG").GetComponent<Image>();
        }

        //Text
        if (text_Panel_ToolTip == null)
        {
            text_Panel_ToolTip = transform.GetComponent<Text>();
        }
        if (text_Title == null)
        {
            text_Title = transform.Find("Img_BG/Text_Title").GetComponent<Text>();
        }
        if (text_Content == null)
        {
            text_Content = transform.Find("Text_Content").GetComponent<Text>();
        }

        //CanvasGroup
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    public override void OnEnter(float dwellTime, int siblingIndex = 0)
    {
        //更新面板内容并显示
        UpdateTipContent();

        //面板动画
        DOTween.To(() => canvasGroup.alpha, alpha => canvasGroup.alpha = alpha, 1f, 1f)
            .OnComplete(()=> {         
                //面板停留时间
                StartCoroutine(Dwell(dwellTime));
            });
    }

    public override void OnExit()
    {
        this.gameObject.SetActive(false);
    }

    public override void OnPause()
    {

    }

    public override void OnResume()
    {

    }


    private void UpdateTipContent()
    {
        //设置，提示框为最顶层
        transform.SetAsLastSibling();

        //设置面板背景，标题，以及内容
        text_Title.text = Manager_Tip.Instance.currentTip.Title;
        text_Panel_ToolTip.text = Manager_Tip.Instance.currentTip.TipContent;
        text_Content.text = Manager_Tip.Instance.currentTip.TipContent;

        //更新背景图片
        if (Manager_Tip.Instance.currentTip.BGImgPath == "")
        {

        }
        else
        {
            Texture2D tex2D = Resources.Load<Texture2D>(Manager_Tip.Instance.currentTip.BGImgPath);
            img_BG.sprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), Vector2.zero);
            img_BG.rectTransform.sizeDelta = new Vector2(tex2D.width, tex2D.height) * 0.1f;
        }

        //显示
        this.gameObject.SetActive(true);
    }



    IEnumerator Dwell(float dwellTime)
    {
        yield return new WaitForSeconds(dwellTime);

        DOTween.To(() => canvasGroup.alpha, alpha => canvasGroup.alpha = alpha, 0f, 1f)
            .OnComplete(() => { Manager_Panel.Instance.PopStack_Panel(); });
    }


    IEnumerator DownloadImg_BG(string path)
    {
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path);
        yield return uwr.SendWebRequest();
        Texture2D tex2D = (uwr.downloadHandler as DownloadHandlerTexture).texture;
        img_BG.sprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), Vector2.zero);
    }
}
