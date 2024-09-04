using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using FTools;
using UnityEngine.SceneManagement;
using System;
using STools.Tool_Manager;
public class Panel_Login : Base_Panel
{
    public InputField input_UserId;//手机号
    public InputField input_Password;//密码

    public Button btn_Login;//登录
    public Button btn_Register;//注册

    private Request_Login request_Login;//登录请求


    void Start()
    {
        btn_Login.onClick.AddListener(OnClickBtn_Login);
        btn_Register.onClick.AddListener(OnClickBtn_Register);

        request_Login = GetComponent<Request_Login>();
        canvasGroup = GetComponent<CanvasGroup>();
    }


    #region Private Method
    private void OnClickBtn_Login()//登录
    {
        //1、判断手机号是否为空
        if (string.IsNullOrEmpty(input_UserId.text))
        {
            //播放音效
            Manager_Audio.Instance.PlayAudioByMusicType(ResType_Music.Button.Login_01, Camera.main.transform.position);

            Manager_Tip.Instance.Update_TipContent((byte)TipKey.IsNullOrEmpty_Username, "手机号不能为空，请输入手机号。");
            Manager_Panel.Instance.PushStack_Panel(Type_Panel.Tip);
            return;
        }

        //2、判断手机号是否输入正确
        if (!Tools.IsMobilePhone(input_UserId.text))
        {
            //播放音效
            Manager_Audio.Instance.PlayAudioByMusicType(ResType_Music.Button.Login_01, Camera.main.transform.position);

            Manager_Tip.Instance.Update_TipContent((byte)TipKey.IsErrorMobilePhone, "请检查手机号是否输入正确。");
            Manager_Panel.Instance.PushStack_Panel(Type_Panel.Tip);
            return;
        }

        //3、判断密码是否为空
        if (string.IsNullOrEmpty(input_Password.text))
        {
            //播放音效
            Manager_Audio.Instance.PlayAudioByMusicType(ResType_Music.Button.Login_01, Camera.main.transform.position);

            Manager_Tip.Instance.Update_TipContent((byte)TipKey.IsNullOrEmpty_Password, "密码不能为空，请输入密码。");
            Manager_Panel.Instance.PushStack_Panel(Type_Panel.Tip);
            return;
        }

        request_Login.userId = input_UserId.text;
        request_Login.password = input_Password.text;

        request_Login.OperationRequest();//发起请求

        #region 测试
        ////清空栈，切换到另一个场景时，上一个场景的面板应该清空
        //Manager_Panel.Instance.ClearAllStackElement();
        ////跳转到下一个场景
        //SceneManager.LoadScene(1);
        #endregion
    }

    private void OnClickBtn_Register()//注册
    {
        Manager_Audio.Instance.PlayAudioByMusicType(ResType_Music.Button.Login_01, Camera.main.transform.position);
        Manager_Panel.Instance.PushStack_Panel(Type_Panel.Register);
    }
    #endregion



    #region Public Method
    public void OnLoginResponse(Code_Return returnCode)//登录响应
    {
        if (returnCode == Code_Return.Success)
        {
            //清空栈，切换到另一个场景时，上一个场景的面板应该清空
            Manager_Panel.Instance.ClearAllStackElement();

            //跳转到下一个场景
            SceneManager.LoadScene(2);
        }
        else
        {
            //播放音效
            Manager_Audio.Instance.PlayAudioByMusicType(ResType_Music.Button.Login_01, Camera.main.transform.position);

            //提示框
            Manager_Tip.Instance.Update_TipContent((byte)TipKey.IsErrorUserNameOrPassword,"用户名或密码错误。");

            //提示面板
            Manager_Panel.Instance.PushStack_Panel(Type_Panel.Tip);
        }
    }
    #endregion



    #region Base Method
    //刚进入该界面时
    public override void OnEnter(float dwellTimeint,int siblingIndex=0)//先暂停之前界面，再进入要显示的界面
    {

    }


    //新界面要出现时，则暂停当前界面
    public override void OnPause()
    {
        canvasGroup.blocksRaycasts = false;
    }


    //界面恢复时调用
    public override void OnResume()
    {
        canvasGroup.blocksRaycasts = true;
    }


    //界面退出时
    public override void OnExit()
    {

    }
    #endregion


}
