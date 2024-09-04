using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTools.Community;
using ProtoBuf;
namespace FTools.Community
{
    [ProtoContract]
    public class Room
    {
        [ProtoMember(1)]
        public virtual Guid Guid { get; set; }//房间ID
        [ProtoMember(2)]
        public virtual RoomType RoomType{ get; set; } = RoomType.Double;//房间类型
        [ProtoMember(3)]
        public virtual RoomState RoomState { get; set; } = RoomState.Normal;//房间状态
        [ProtoMember(4)]
        public virtual string Room_name { get; set; }//房间名称
        [ProtoMember(5)]
        public virtual long Room_owner { get; set; }//房主
        [ProtoMember(6)]
        public virtual List<User> Room_players { get; set; }//房间内的玩家
    }
}