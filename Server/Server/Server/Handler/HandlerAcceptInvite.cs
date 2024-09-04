using FTools.Codes;
using FTools.Community;
using FTools.Utils;
using Photon.SocketServer;
using System;
using System.Collections.Generic;

namespace Server.Handler
{
    public class HandlerAcceptInvite : BaseHandler
    {
        public HandlerAcceptInvite()
        {
            operationCode = OperationCode.AcceptInvite;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, Peer peer)
        {
            byte[] bytesRoomGuid = (byte[])Tool.GetValue(operationRequest.Parameters, (byte)ParameterCode.Room);
            Guid roomGuid = Tool.DeSerializeByProtobufNet<Guid>(bytesRoomGuid);
            User tempUser = Tool.DeSerializeByProtobufNet<User>((byte[])Tool.GetValue(operationRequest.Parameters, (byte)ParameterCode.User));
            if (tempUser != null)
            {
                Room tempRoom = null;
                foreach (Room room in Server.Instance.listRooms)
                {
                    if (room.Guid.CompareTo(roomGuid) == 0)
                    {
                        tempRoom = room;
                        break;
                    }
                }
                if (tempRoom != null)
                {
                    tempRoom.Room_players.Add(tempUser);
                }

                byte[] bytesRoom = Tool.SerializeByProtobufNet(tempRoom);
                EventData ed = new EventData((byte)EventCode.NewPlayerJoinRoom);
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ParameterCode.Room, bytesRoom);
                ed.Parameters = data;

                foreach (User user in tempRoom.Room_players)
                {
                    foreach (Peer tempPeer in Server.Instance.listPeer)
                    {
                        if (tempPeer.user.Id == user.Id)
                        {
                            tempPeer.SendEvent(ed, sendParameters);
                        }
                    }
                }
                Server.log.Info("我已经向该房间内的所有玩家发送当前的房间信息（因为已经有新的玩家加入了）");
            }
        }
    }
}
