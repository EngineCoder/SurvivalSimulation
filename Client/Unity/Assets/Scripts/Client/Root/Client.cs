using System;
using UnityEngine;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using FTools;
using FTools.Tools;
using ProtoBuf;

public class Client : SingleMode<Client>, IPhotonPeerListener
{
    private static PhotonPeer peer;
    public static PhotonPeer Peer
    {
        get { return peer; }
        set { peer = value; }
    }

    public static User user;//存储当前用户信息
    public static long ID;//存储当前用户ID
    public static string userId;


    /// <summary>
    /// 操作码
    /// </summary>
    private Dictionary<Code_Operation, BaseRequest> dict_Requset = new Dictionary<Code_Operation, BaseRequest>();


    /// <summary>
    /// 事件码
    /// </summary>17699
    private Dictionary<Code_Event, BaseEvent> dict_Event = new Dictionary<Code_Event, BaseEvent>();


    void Awake()
    {
        peer = new PhotonPeer(this, ConnectionProtocol.Udp);
        peer.Connect("127.0.0.1:5055", "Survival");
        peer.DisconnectTimeout = 6000;

        //序列化，User对象
        //PhotonPeer.RegisterType(typeof(User), (byte)'U', Tool_Serialize.Serialize_ReturnByteArray, Tool_Serialize.DeSerialize(typeof(byte[])));
    }


    // Update is called once per frame
    void Update()
    {
        peer.Service();
    }


    new void OnDestroy()
    {
        base.OnDestroy();
        if (Peer != null && Peer.PeerState == PeerStateValue.Disconnected)
        {
            Peer.Disconnect();
        }
    }


    public void DebugReturn(DebugLevel level, string message)
    {

    }


    /// <summary>
    /// 服务器向客户端发送的事件请求
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEvent(EventData eventData)
    {
        Code_Event eventCode = (Code_Event)eventData.Code;
        BaseEvent baseEvent = Tool_Dict.GetValue(dict_Event, eventCode);
        baseEvent.OnEvent(eventData);
    }


    /// <summary>
    /// 客户端向服务器发送请求之后，服务器反馈给客户端的方法
    /// </summary>
    /// <param name="operationResponse"></param>
    public void OnOperationResponse(OperationResponse operationResponse)
    {
        Code_Operation operationCode = (Code_Operation)operationResponse.OperationCode;

        BaseRequest request = null;

        bool temp = dict_Requset.TryGetValue(operationCode, out request);

        if (temp)
        {
            request.OnOperationResponse(operationResponse);
        }
        else
        {
            Debug.Log("<color=red>没有找到对应的请求响应</color>");
        }
    }

    /// <summary>
    /// 客户端的连接状态
    /// </summary>
    /// <param name="statusCode"></param>
    public void OnStatusChanged(StatusCode statusCode)
    {
        //Debug.Log(statusCode);
    }



    /// <summary>
    /// 添加请求
    /// </summary>
    /// <param name="request"></param>
    public void AddRequest(BaseRequest request)
    {
        dict_Requset.Add(request.operationCode, request);
    }


    /// <summary>
    /// 移除请求
    /// </summary>
    /// <param name="request"></param>
    public void RemoveRequest(BaseRequest request)
    {
        dict_Requset.Remove(request.operationCode);
    }


    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="request"></param>
    public void AddEvent(BaseEvent baseEvent)
    {
        dict_Event.Add(baseEvent.eventCode, baseEvent);
    }


    /// <summary>
    /// 移除事件
    /// </summary>
    /// <param name="request"></param>
    public void RemoveEvent(BaseEvent baseEvent)
    {
        dict_Event.Remove(baseEvent.eventCode);
    }
}


[ProtoContract]
public class User
{
    [ProtoMember(1)]
    public virtual long Id { get; set; }
    [ProtoMember(2)]
    public virtual string User_id { get; set; }
    [ProtoMember(3)]
    public virtual string User_nickname { get; set; }
    [ProtoMember(4)]
    public virtual string User_password { get; set; }
    [ProtoMember(5)]
    public virtual int User_sex { get; set; }

    [ProtoMember(6)]
    public virtual int User_copper_coins { get; set; }//铜钱
    [ProtoMember(7)]
    public virtual int User_gold_ingot { get; set; }//元宝

    [ProtoMember(8)]
    public virtual string User_integral_grade { get; set; }//积分等级
    [ProtoMember(9)]
    public virtual int User_innings { get; set; }//总局数
    [ProtoMember(10)]
    public virtual int User_win_rate { get; set; }//胜率

    [ProtoMember(11)]
    public virtual DateTime User_lastLogin_date { get; set; }//最后一次登录
    [ProtoMember(12)]
    public virtual DateTime User_register_date { get; set; }//注册日期
}
