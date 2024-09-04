using FTools.Codes;
using FTools.Community;
using FTools.Tools;
using Photon.SocketServer;
using System;
using System.Collections.Generic;

namespace Server.Handler
{
    public class HandlerRefuseMatching : BaseHandler
    {
        public HandlerRefuseMatching()
        {
            operationCode = OperationCode.MatchingRefuse;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, Peer peer)
        {
            byte[] byteRoomGuid = (byte[])Tool.GetValue(operationRequest.Parameters, (byte)ParameterCode.Room);
            Guid roomGuid = Tool.DeSerializeByProtobufNet<Guid>(byteRoomGuid);
            List<User> users = null;
            foreach (Room item in Server.Instance.listRooms)
            {
                if (roomGuid.CompareTo(item.Guid)==0)
                {
                    users = item.Room_players;
                    item.RoomState = RoomState.Normal;//更新房间状态
                    break;
                }
            }
            if (users != null)
            {
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ParameterCode.MatchState, RoomState.MatchingRefuse);
                EventData ed = new EventData((byte)(EventCode.MatchingRefuse));
                ed.Parameters = data;
                foreach (User user in users)
                {
                    foreach (Peer peer1 in Server.Instance.listPeer)
                    {
                        if (peer1.user.Id == user.Id)
                        {
                            peer1.SendEvent(ed, sendParameters);
                            break;
                        }
                    }
                }
                Server.log.Info("在匹配时已拒绝，请稍后！");
            }
        }
    }
}
