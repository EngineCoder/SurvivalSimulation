using FTools.Codes;
using FTools.Community;
using FTools.Utils;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler
{
    public class HandlerRefuseMatched : BaseHandler
    {
        public HandlerRefuseMatched()
        {
            operationCode = OperationCode.MatchedRefuse;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, Peer peer)
        {
            long userID = (long)Tool.GetValue(operationRequest.Parameters, (byte)ParameterCode.ID);
            byte[] bigRoomIDBytes = (byte[])Tool.GetValue(operationRequest.Parameters, (byte)ParameterCode.BigRoom);
            Guid bigRoomID = Tool.DeSerializeByProtobufNet<Guid>(bigRoomIDBytes);
            if (Server.Instance.bigRooms.ContainsKey(bigRoomID))
            {
                BigRoom tempBigRoom = Server.Instance.bigRooms[bigRoomID];//根据大房间ID找到该房间);
                EventData ed = new EventData((byte)EventCode.MatchedRefuse, null);
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
                tempBigRoom.ATeam.Clear();
                tempBigRoom.BTeam.Clear();
                tempBigRoom.acceptCount = 0;
                Server.log.Info("房间已清理");
                tempBigRoom.isHaveRefused = true;
                Server.Instance.bigRooms.Remove(bigRoomID);
            }
        }
    }
}
