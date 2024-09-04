using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTools.Codes
{
    /// <summary>
    /// 参数码：区分传送的参数是什么数据
    /// </summary>
    public enum ParameterCode : byte
    {
        /// <summary>
        /// Id
        /// </summary>
        ID = 0,

        /// <summary>
        /// User
        /// </summary>
        User = 1,

        /// <summary>
        /// 账号
        /// </summary>
        Account = 2,

        /// <summary>
        /// 密码
        /// </summary>
        Password = 3,

        /// <summary>
        /// 注册日期
        /// </summary>
        RegisterData = 4,


        /// <summary>
        /// 房间信息
        /// </summary>
        Room = 5,
        /// <summary>
        /// 大房间
        /// </summary>
        BigRoom = 6,
        /// <summary>
        /// 房间类型
        /// </summary>
        RoomType = 7,
        /// <summary>
        /// 房主
        /// </summary>
        RoomOwner = 8,
        /// <summary>
        /// 昵称
        /// </summary>
        Nickname = 9,
        /// <summary>
        /// 邀请时的玩家昵称集合
        /// </summary>
        Nicknames = 10,
        /// <summary>
        /// 匹配状态
        /// </summary>
        MatchState = 11,


        //位置同步
        X, Y, Z,

        /// <summary>
        /// 玩家userid列表
        /// </summary>
        UserIdList,


        /// <summary>
        /// Player Data List
        /// </summary>
        PlayerDataList,

        /// <summary>
        /// The Event Code.
        /// </summary>
        EventCode = 60,

        /// <summary>
        /// The Username.
        /// </summary>
        Username = 91,

        /// <summary>
        /// The Old Position.
        /// </summary>
        OldPosition = 92,

        /// <summary>
        /// The Position.
        /// </summary>
        Position = 93,

        /// <summary>
        /// The Properties.
        /// </summary>
        Properties = 94,

        /// <summary>
        /// The Item Id.
        /// </summary>
        ItemId = 95,

        /// <summary>
        /// The Item Type.
        /// </summary>
        ItemType = 96,

        /// <summary>
        /// The properties revision.
        /// </summary>
        PropertiesRevision = 97,

        /// <summary>
        /// The custom event code.
        /// </summary>
        CustomEventCode = 98,

        /// <summary>
        /// The event data.
        /// </summary>
        EventData = 99,

        /// <summary>
        /// The top left corner.
        /// </summary>
        TopLeftCorner = 100,

        /// <summary>
        /// The tile dimensions.
        /// </summary>
        TileDimensions = 101,

        /// <summary>
        /// The bottom right corner.
        /// </summary>
        BottomRightCorner = 102,

        /// <summary>
        /// The world name.
        /// </summary>
        WorldName = 103,

        /// <summary>
        /// The view distance.
        /// </summary>
        ViewDistanceEnter = 104,

        /// <summary>
        /// The properties set.
        /// </summary>
        PropertiesSet = 105,

        /// <summary>
        /// The properties unset.
        /// </summary>
        PropertiesUnset = 106,

        /// <summary>
        /// The event reliability.
        /// </summary>
        EventReliability = 107,

        /// <summary>
        /// The event receiver.
        /// </summary>
        EventReceiver = 108,

        /// <summary>
        /// The subscribe.
        /// </summary>
        Subscribe = 109,

        /// <summary>
        /// The view distance exit.
        /// </summary>
        ViewDistanceExit = 110,

        /// <summary>
        /// The interest area id.
        /// </summary>
        InterestAreaId = 111,

        /// <summary>
        /// The counter receive interval.
        /// </summary>
        CounterReceiveInterval = 112,

        /// <summary>
        /// The counter name.
        /// </summary>
        CounterName = 113,

        /// <summary>
        /// The counter time stamps.
        /// </summary>
        CounterTimeStamps = 114,

        /// <summary>
        /// The counter values.
        /// </summary>
        CounterValues = 115,

        /// <summary>
        /// The current rotation.
        /// </summary>
        Rotation = 116,

        /// <summary>
        /// The previous rotation.
        /// </summary>
        OldRotation = 117
    }
}