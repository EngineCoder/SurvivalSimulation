using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using FTools;
using System;

public class Request_Register : BaseRequest
{

    [HideInInspector]
    public string userid;
    [HideInInspector]
    public string password;

    private Panel_Register panel_Register;

    public override void Start()
    {
        base.Start();
        panel_Register = GetComponent<Panel_Register>();
    }

    public override void OperationRequest()
    {
        Dictionary<byte, object> data = new Dictionary<byte, object>();
        data.Add((byte)Code_Parameter.UserID, userid);
        data.Add((byte)Code_Parameter.Password, password);

        //data.Add((byte)Code_Parameter.RegisterData, DateTime.Now.ToString());

        //客户端发起请求
        Client.Peer.SendOperation((byte)operationCode, data,SendOptions.SendReliable);
    }

    //服务器反馈给客户端结果
    public override void OnOperationResponse(OperationResponse operationResponse)
    {
        Code_Return returnCode = (Code_Return)operationResponse.ReturnCode;

        panel_Register.OnRegisterResponse(returnCode);
    }
}
