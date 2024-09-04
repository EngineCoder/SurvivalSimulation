using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTools.Codes
{
    /// <summary>
    /// 请求-响应类型同一码
    /// </summary>
    public enum OperationCode
    {
        /// <summary>
        /// 无操作
        /// </summary>
        Nil = 0,
        /// <summary>
        /// 登录
        /// </summary>
        Login = 1,
        /// <summary>
        /// 注册
        /// </summary>
        Register = 2,

        //========================================Room======================================
        /// <summary>
        /// 创建房间
        /// </summary>
        CreateRoom = 3,
        /// <summary>
        /// 加入房间
        /// </summary>
        JoinRoom = 4,
        /// <summary>
        /// 离开房间
        /// </summary>
        LeaveRoom = 5,
        /// <summary>
        /// 输入昵称进行查询筛选操作
        /// </summary>
        SelectPlayer = 6,
        /// <summary>
        /// 向筛选后的哪些玩家发送邀请
        /// </summary>
        SendInvite = 7,
        /// <summary>
        /// 接受邀请
        /// </summary>
        AcceptInvite = 8,

        //========================================寻找匹配==================================
        /// <summary>
        /// 寻找对局
        /// </summary>
        FindMatch = 9,
        /// <summary>
        /// 匹配中拒绝
        /// </summary>
        MatchingRefuse = 10,
        /// <summary>
        /// 匹配到拒绝
        /// </summary>
        MatchedRefuse = 11,
        /// <summary>
        /// 接受匹配
        /// </summary>
        MatchedAccept = 12,
        //===================================================================================
        SyncPosition,
        SyncPlayers,
        CreateWorld = 90,
        EnterWorld = 91,
        ExitWorld = 92,
        Move = 93,
        RaiseGenericEvent = 94,
        SetProperties = 95,

        /// <summary>
        /// The spawn item.
        /// </summary>
        SpawnItem = 96,

        /// <summary>
        /// The destroy item.
        /// </summary>
        DestroyItem = 97,

        /// <summary>
        /// The subscribe item.
        /// </summary>
        SubscribeItem = 98,

        /// <summary>
        /// The unsubscribe item.
        /// </summary>
        UnsubscribeItem = 99,

        /// <summary>
        /// The set view distance.
        /// </summary>
        SetViewDistance = 100,

        /// <summary>
        /// The attach interest area.
        /// </summary>
        AttachInterestArea = 101,

        /// <summary>
        /// The detach interest area.
        /// </summary>
        DetachInterestArea = 102,

        /// <summary>
        /// The add interest area.
        /// </summary>
        AddInterestArea = 103,

        /// <summary>
        /// The remove interest area.
        /// </summary>
        RemoveInterestArea = 104,

        /// <summary>
        /// The get properties.
        /// </summary>
        GetProperties = 105,

        /// <summary>
        /// The move interest area.
        /// </summary>
        MoveInterestArea = 106,

        /// <summary>
        /// The radar subscribe.
        /// </summary>
        RadarSubscribe = 107,

        /// <summary>
        /// The unsubscribe counter.
        /// </summary>
        UnsubscribeCounter = 108,

        /// <summary>
        /// The subscribe counter.
        /// </summary>
        SubscribeCounter = 109
    }
}
