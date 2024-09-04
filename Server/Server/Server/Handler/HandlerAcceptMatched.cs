using FTools.Codes;
using FTools.Community;
using FTools.Tools;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler
{
    public class HandlerAcceptMatched : BaseHandler
    {
        public HandlerAcceptMatched()
        {
            operationCode = OperationCode.MatchedAccept;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, Peer peer)
        {
            long userID = (long)Tool.GetValue(operationRequest.Parameters, (byte)ParameterCode.ID);
            byte[] bigRoomIDBytes = (byte[])Tool.GetValue(operationRequest.Parameters, (byte)ParameterCode.BigRoom);
            Guid bigRoomID = Tool.DeSerializeByProtobufNet<Guid>(bigRoomIDBytes);
            if (Server.Instance.bigRooms.ContainsKey(bigRoomID))//如果不存在该I房间ID则不作操作
            {
                BigRoom tempBigRoom = Server.Instance.bigRooms[bigRoomID];//根据大房间ID找到该房间
                tempBigRoom.acceptCount += 1;//接受。更新该大房间的接受人数
                if (tempBigRoom.acceptCount == 10)//通知所有客户端进入游戏加载界面
                {
                    byte[] bigRoomBytes = Tool.SerializeByProtobufNet(tempBigRoom);
                    Dictionary<byte, object> data = new Dictionary<byte, object>();
                    data.Add((byte)ParameterCode.MatchState, RoomState.MatchedFinished);
                    data.Add((byte)ParameterCode.BigRoom, bigRoomBytes);
                    EventData ed = new EventData((byte)EventCode.MatchedFinished, data);
                    foreach (User item in tempBigRoom.ATeam)
                    {
                        foreach (Peer temp in Server.Instance.listPeer)
                        {
                            if (temp.user.Id == item.Id)
                            {
                                temp.SendEvent(ed, sendParameters);
                            }
                        }
                    }
                    foreach (User item in tempBigRoom.BTeam)
                    {
                        foreach (Peer temp in Server.Instance.listPeer)
                        {
                            if (temp.user.Id == item.Id)
                            {
                                temp.SendEvent(ed, sendParameters);
                            }
                        }
                    }
                }
            }
        }
    }
}