﻿using ProtoBuf;
using System;
using System.Collections.Generic;

namespace FTools.Community
{
    [ProtoContract]
    public class BigRoom
    {
        [ProtoMember(1)]
        public virtual Guid BigRoomID { get; set; }//房间ID
        [ProtoMember(2)]
        public virtual int acceptCount { get; set; }//接受计数
        [ProtoMember(3)]
        public virtual List<User> ATeam { get; set; }//A队
        [ProtoMember(4)]
        public virtual List<User> BTeam { get; set; }//B队
        [ProtoMember(5)]
        public virtual bool isHaveRefused { get; set; }//已经拒绝
    }
}
