using FTools.Codes;
using FTools.Community;
using FTools.Utils;
using Photon.SocketServer;
using System;
using System.Collections.Generic;

namespace Server.Handler
{
    public class HandlerInvite : BaseHandler
    {
        public HandlerInvite()
        {
            operationCode = OperationCode.SendInvite;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, Peer peer)
        {
            byte[] nicknamesByte = (byte[])Tool.GetValue(operationRequest.Parameters, (byte)ParameterCode.Nicknames);
            List<string> nicknames = Tool.DeSerializeByProtobufNet<List<string>>(nicknamesByte);
            byte[] byteRoomGuid = (byte[])Tool.GetValue(operationRequest.Parameters, (byte)ParameterCode.Room);
            Guid roomGuid = Tool.DeSerializeByProtobufNet<Guid>(byteRoomGuid);//房间ID
            string userNickname = (string)Tool.GetValue(operationRequest.Parameters, (byte)ParameterCode.Nickname);//邀请人昵称
            if (nicknames != null)
            {
                EventData ed = new EventData((byte)EventCode.InviteJoin);
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ParameterCode.Room, byteRoomGuid);
                data.Add((byte)ParameterCode.Nickname, userNickname);
                ed.Parameters = data;
                foreach (string strNickname in nicknames)
                {
                    foreach (Peer item in Server.Instance.listPeer)
                    {
                        User tempUser = item.user;
                        if (strNickname == tempUser.User_nickname && (peer.user.Id != item.user.Id))//不能向自己发送邀请
                        {
                            if (item.room == null || item.room.Guid.CompareTo(roomGuid) != 0)//如果被邀请的人，不在邀请人的房间里（可在别的房间里也可以不在房间里）
                            {
                                item.SendEvent(ed, sendParameters);//向被邀请的玩家发送消息
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
