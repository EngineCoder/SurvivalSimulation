using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using FTools;

using STools.Tool_Manager;

public class Panel_Register : Base_Panel
{

    #region UI

    public Button btn_Return;
    public Button btn_Register;

    public InputField input_UserId;
    public InputField input_Password;

    #endregion


    #region  Private Attributes
    private Request_Register request_Register;
    #endregion


    void Start()
    {
        btn_Return.onClick.AddListener(OnClickBtnReturn);
        btn_Register.onClick.AddListener(OnClickBtnRegister);

        request_Register = GetComponent<Request_Register>();

        if (canvasGroup == null) { canvasGroup = GetComponent<CanvasGroup>(); }
    }

    private void OnClickBtnRegister()
    {
        Manager_Audio.Instance.PlayAudioByMusicType(ResType_Music.Button.Login_01, Camera.main.transform.position);

        //1、判断手机号是否为空
        if (string.IsNullOrEmpty(input_UserId.text))
        {
            Manager_Tip.Instance.Update_TipContent((byte)TipKey.IsNullOrEmpty_Username, "手机号不能为空，请输入手机号。");
            Manager_Panel.Instance.PushStack_Panel(Type_Panel.Tip);
            return;
        }

        //2、判断手机号是否输入正确
        if (!Tools.IsMobilePhone(input_UserId.text))
        {
            Manager_Tip.Instance.Update_TipContent((byte)TipKey.IsErrorMobilePhone, "请检查手机号是否输入正确。");
            Manager_Panel.Instance.PushStack_Panel(Type_Panel.Tip);
            return;
        }

        //3、判断密码是否为空
        if (string.IsNullOrEmpty(input_Password.text))
        {
            Manager_Tip.Instance.Update_TipContent((byte)TipKey.IsNullOrEmpty_Password, "密码不能为空，请输入密码。");
            Manager_Panel.Instance.PushStack_Panel(Type_Panel.Tip);
            return;
        }

        //3、判断密码是否为空
        if (input_Password.text.Length < 6)
        {
            Manager_Tip.Instance.Update_TipContent((byte)TipKey.IsNullOrEmpty_Password, "密码不能少于6位。");
            Manager_Panel.Instance.PushStack_Panel(Type_Panel.Tip);
            return;
        }

        request_Register.userid = input_UserId.text;
        request_Register.password = input_Password.text;

        request_Register.OperationRequest();
    }


    private void OnClickBtnReturn()
    {
        Manager_Audio.Instance.PlayAudioByMusicType(ResType_Music.Button.Login_01, Camera.main.transform.position);
        Manager_Panel.Instance.PopStack_Panel();
    }


    public void OnRegisterResponse(Code_Return returnCode)
    {
        if (returnCode == Code_Return.Success)
        {
            Debug.Log("成功");

            //"注册成功，请返回登录！";
            Manager_Tip.Instance.Update_TipContent((byte)TipKey.IsSuccessfulRegister, "注册成功，请返回登录！");
            Manager_Panel.Instance.PopStack_Panel();
        }
        else
        {
            Debug.Log("失败");

            //"注册失败，请重新注册！";
            Manager_Tip.Instance.Update_TipContent((byte)TipKey.IsErrorUserIdAlreadyRegister, "注册失败，该手机号已注册");
            Manager_Panel.Instance.PushStack_Panel(Type_Panel.Tip);
        }
    }



    public override void OnPause()
    {
        canvasGroup.blocksRaycasts = false;
    }

    public override void OnEnter(float dwellTime = 2f, int siblingIndex = 0)//先暂停之前界面，再进入要显示的界面
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
