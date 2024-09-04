using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using FTools;
using FTools.Tools;
using STools.Tool_Manager;
public class Request_Login : BaseRequest
{
    [HideInInspector]
    public string userId;

    [HideInInspector]
    public string password;


    private Panel_Login panel_Login;


    public override void Start()
    {
        base.Start();
        panel_Login = GetComponent<Panel_Login>();
    }


    public override void OperationRequest()
    {
        //添加向字典中，添加数据
        Dictionary<byte, object> data = new Dictionary<byte, object>();
        data.Add((byte)Code_Parameter.UserID, userId);
        data.Add((byte)Code_Parameter.Password, password);

        //设置好请求码、请求数据,向服务器发送请求
        Client.Peer.SendOperation((byte)operationCode, data, SendOptions.SendReliable);
    }


    public override void OnOperationResponse(OperationResponse operationResponse)
    {
        Code_Return returnCode = (Code_Return)operationResponse.ReturnCode;

        if (returnCode == Code_Return.Success)
        {
            try
            {
                User u = Tool_Serialize.DeSerialize<User>((byte[])operationResponse.Parameters[(byte)Code_Parameter.User]);
                Client.userId = u.User_id;
                Client.user = u;
            }
            catch (System.Exception e)
            {
                Manager_Tip.Instance.Update_TipContent(101, e.Message);
                Manager_Panel.Instance.PushStack_Panel(Type_Panel.Tip, 10);
            }
        }
        panel_Login.OnLoginResponse(returnCode);
    }
}
