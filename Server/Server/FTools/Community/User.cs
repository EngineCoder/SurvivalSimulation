using ProtoBuf;
using System;

namespace FTools.Community
{
    [ProtoContract]
    public class User
    {
        [ProtoMember(1)]
        public virtual long Id { get; set; }
        [ProtoMember(2)]
        public virtual string User_account { get; set; }
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
}
