using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDKShare : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        //ssdk = GameObject.Find("Root").GetComponent<ShareSDK>();
        //ssdk.authHandler = OnAuthResultHandler;//登录验证
        //ssdk.showUserHandler = OnGetUserInfoResultHandler;//用户信息
    }

    /// <summary>
    /// QQ 登录
    /// </summary>
    void OnClickBtn_QQ()
    {
        //ssdk.Authorize(PlatformType.QQ);
    }

    /// <summary>
    /// 验证结果处理器
    /// </summary>
    /// <param name="reqID">请求ID</param>
    /// <param name="responseState">响应状态</param>
    /// <param name="platformType"></param>
    /// <param name="result"></param>
    //void OnAuthResultHandler(int reqID, ResponseState responseState, PlatformType platformType, Hashtable result)
    //{
    //    if (responseState == ResponseState.Success)
    //    {
    //        ////授权成功的话，获取用户信息
    //        //ssdk.GetUserInfo(platformType);

    //        Hashtable hashtable = ssdk.GetAuthInfo(platformType);
    //        JsonData jd = JsonMapper.ToObject(MiniJSON.jsonEncode(hashtable));
    //        int gender = 0;
    //        if (jd["userGender"].ToString() == "m")
    //        {
    //            gender = 1;
    //        }
    //        //ShowEveryMessage.Instance.ShowToolTip(jd["userID"].ToString() + "【】" + jd["userName"].ToString() + "【】 " + jd["userIcon"].ToString()+"头像链接长度为："+jd["userIcon"].ToString().Length + "【】" + gender.ToString());
    //        switch (platformType)
    //        {

    //            case PlatformType.WeChat:
    //                break;
    //            case PlatformType.QQ:
    //                break;
    //        }
    //    }
    //    else if (responseState == ResponseState.Fail)
    //    {
    //        print("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
    //    }
    //    else if (responseState == ResponseState.Cancel)
    //    {
    //        print("cancel!");
    //    }
    //}

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="reqID"></param>
    /// <param name="state"></param>
    /// <param name="type"></param>
    /// <param name="result"></param>
    //void OnGetUserInfoResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    //{
    //    if (state == ResponseState.Success)
    //    {
    //        ssdk.GetUserInfo(type);
    //    }
    //    else if (state == ResponseState.Fail)
    //    {
    //        //ShowEveryMessage.Instance.ShowToolTip("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
    //    }
    //    else if (state == ResponseState.Cancel)
    //    {
    //        //ShowEveryMessage.Instance.ShowToolTip("cancel !");
    //    }
    //}

    //获取好友信息
//    void OnFollowFriendResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
//    {
//        if (state == ResponseState.Success)
//        {
//            //授权成功的话，获取用户信息
//            Hashtable hashtable = ssdk.GetAuthInfo(type);
//            jd = JsonMapper.ToObject(MiniJSON.jsonEncode(hashtable));
//            //UserInfo userInfo = new UserInfo(jd["userID"].ToString(), jd["userName"].ToString(), jd["userIcon"].ToString(),jd["userGender"].ToString());
//            gender = 0;
//            if (jd["userGender"].ToString() == "m")
//            {
//                gender = 1;
//            }
//            //ShowEveryMessage.Instance.ShowToolTip(jd["userID"].ToString() + " " + jd["userName"].ToString() + " " + jd["userIcon"].ToString() + " " + gender.ToString());
//        }
//        else if (state == ResponseState.Fail)
//        {
//#if UNITY_ANDROID
//            print("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
//#elif UNITY_IPHONE
//			print ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
//#endif
//        }
//        else if (state == ResponseState.Cancel)
//        {

//        }
//    }

    /// <summary>
    /// 匹配手机号
    /// </summary>
    /// <param name="str_handset"></param>
    /// <returns></returns>
    public bool IsHandset(string str_handset)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(str_handset, @"^[1]+[3,4,5,7,8,9]+\d{9}");
    }
}
