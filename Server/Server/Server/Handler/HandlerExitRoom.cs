using FTools.Codes;
using FTools.Community;
using FTools.Tools;
using Photon.SocketServer;
using System;
using System.Collections.Generic;

namespace Server.Handler
{
    public class HandlerExitRoom : BaseHandler
    {
        public HandlerExitRoom()
        {
            operationCode = OperationCode.LeaveRoom;
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, Peer peer)
        {
            Guid roomGuid = Tool.DeSerializeByProtobufNet<Guid>((byte[])operationRequest.Parameters[(byte)ParameterCode.Room]);
            long userID = (long)operationRequest.Parameters[(byte)ParameterCode.ID];

            List<User> users = null;
            Room room = null;
            bool isRoomOwner = false;

            foreach (Room item in Server.Instance.listRooms)
            {
                if (roomGuid.CompareTo(item.Guid) == 0)
                {
                    users = item.Room_players;
                    room = item;
                    break;
                }
            }
            if (users != null)
            {
                User user = null;
                foreach (User userTemp in users)
                {
                    if (userTemp.Id == userID)
                    {
                        user = userTemp;
                        if (user.Id == room.Room_owner)
                        {
                            isRoomOwner = true;
                        }
                        break;
                    }
                }

                OperationResponse response = new OperationResponse(operationRequest.OperationCode);
                response.ReturnCode = (short)ReturnCode.Success;
                peer.SendOperationResponse(response, sendParameters);//向离开的玩家发送可以离开响应

                users.Remove(user);//将要退出房间的玩家移除该房间
                Server.log.Info("==========" + users.Count);
                if (isRoomOwner)//如果退出的是房主，则设置其他人为房主。
                {
                    if (users.Count > 0)
                    {
                        room.Room_owner = users[0].Id;
                    }
                    else//最后一个离开的肯定是房主，则移除该房间，直接退出
                    {
                        Server.Instance.listRooms.Remove(room);
                        Server.log.Info("当前房间的最后一个玩家离开了！");
                        return;
                    }
                }

                byte[] roomBytes = Tool.SerializeByProtobufNet(room);
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ParameterCode.Room, roomBytes);
                EventData ed = new EventData((byte)(EventCode.LeaveRoom));
                ed.Parameters = data;
                foreach (User userTemp in users)
                {
                    foreach (Peer peerTemp in Server.Instance.listPeer)
                    {
                        if (userTemp.Id == peerTemp.user.Id)
                        {
                            peerTemp.SendEvent(ed, sendParameters);
                        }
                    }
                }
                Server.log.Info("有玩家离开了房间！");
            }
        }
    }
}
