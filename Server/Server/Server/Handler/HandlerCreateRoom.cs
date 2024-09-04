using FTools.Codes;
using FTools.Community;
using FTools.Tools;
using Photon.SocketServer;
using System;
using System.Collections.Generic;

namespace Server.Handler
{
    public class HandlerCreateRoom : BaseHandler
    {
        public HandlerCreateRoom()
        {
            operationCode = OperationCode.CreateRoom;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, Peer peer)
        {

            //接收客户端发送来的数据
            RoomType roomType = (RoomType)(Tool.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoomType));
            long roomOwner = (long)(Tool.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoomOwner));
            byte[] bytesUser = (byte[])Tool.GetValue(operationRequest.Parameters, (byte)ParameterCode.User);
            User user= Tool.DeSerializeByProtobufNet<User>(bytesUser);
            Room room = new Room();//创建房间
            room.Guid = Guid.NewGuid();
            room.RoomType = roomType;
            room.RoomState = RoomState.Normal;
            room.Room_owner = roomOwner;
            room.Room_players = new List<User>();
            room.Room_players.Add(user);

            switch (roomType)
            {
                case RoomType.Double:
                    room.Room_name = "核武-单排双排";
                    break;
                case RoomType.Flexible:
                    room.Room_name = "核武-灵活排位";
                    break;
                default:
                    room.Room_name = "核武-单排双排";
                    break;
            }
            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            if (room != null)
            {
                Server.Instance.listRooms.Add(room);
                byte[] bytes_Room = Tool.SerializeByProtobufNet(room);
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ParameterCode.Room, bytes_Room);
                response.Parameters = data;
                response.ReturnCode = 0;//成功
                peer.room = room;//保存到当前的对等体的房间信息
            }
            else
            {
                response.Parameters = null;
                response.ReturnCode = 1;//失败
            }
            peer.SendOperationResponse(response, sendParameters);
        }
    }
}
