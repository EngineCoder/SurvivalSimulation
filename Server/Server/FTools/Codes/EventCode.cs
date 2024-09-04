using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTools.Codes
{
    public enum EventCode : byte
    {
        /// <summary>
        /// 邀请加入
        /// </summary>
        InviteJoin = 0,
        /// <summary>
        /// 新的玩家加入
        /// </summary>
        NewPlayerJoinRoom = 1,


        /// <summary>
        /// 匹配中
        /// </summary>
        Matching = 2,

        /// <summary>
        /// 匹配中拒绝
        /// </summary>
        MatchingRefuse = 3,

        /// <summary>
        /// 已经匹配到
        /// </summary>
        Matched = 4,

        /// <summary>
        /// 匹配到之后的拒绝
        /// </summary>
        MatchedRefuse = 5,

        /// <summary>
        /// 匹配已完成，进入游戏加载界面
        /// </summary>
        MatchedFinished = 6,

        /// <summary>
        /// 加载场景资源完毕，进入游戏
        /// </summary>
        EnterGame = 7,

        /// <summary>
        /// 离开房间
        /// </summary>
        LeaveRoom = 8,


        /// <summary>
        /// 创新玩家
        /// </summary>
        NewPlayer,
        /// <summary>
        /// 同步位置
        /// </summary>
        SyncPosition,
        /// <summary>
        /// 销毁的物品
        /// </summary>
        ItemDestroyed,
        /// <summary>
        /// 物品移动
        /// </summary>
        ItemMoved,
        /// <summary>
        /// 物品属性设置
        /// </summary>
        ItemPropertiesSet,
        /// <summary>
        /// 通用物品
        /// </summary>
        ItemGeneric,
        /// <summary>
        /// 退出世界
        /// </summary>
        WorldExited,
        /// <summary>
        /// 物品订阅
        /// </summary>
        ItemSubscribed,
        /// <summary>
        /// 取消订阅的商品
        /// </summary>
        ItemUnsubscribed,
        /// <summary>
        /// 商品属性
        /// </summary>
        ItemProperties,
        /// <summary>
        /// 雷达更新
        /// </summary>
        RadarUpdate,
        /// <summary>
        /// 计数器数据
        /// </summary>
        CounterData
    }
}
