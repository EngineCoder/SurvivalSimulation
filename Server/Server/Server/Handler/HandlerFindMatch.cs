using FTools.Codes;
using FTools.Community;
using FTools.Utils;
using Photon.SocketServer;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler
{
    public class HandlerFindMatch : BaseHandler
    {
        public HandlerFindMatch()
        {
            operationCode = OperationCode.FindMatch;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, Peer peer)
        {
            byte[] bytesRoomID = (byte[])operationRequest.Parameters[(byte)ParameterCode.Room];
            Guid roomID = Tool.DeSerializeByProtobufNet<Guid>(bytesRoomID);

            List<User> users = null;
            foreach (Room room in Server.Instance.listRooms)//查找房间
            {
                if (roomID.CompareTo(room.Guid) == 0)
                {
                    users = room.Room_players;//当前房间里的玩家
                    room.RoomState = RoomState.Matching;//更新房间状态
                    Server.Instance.queueMaching.Enqueue(room);//将该房间放入匹配队列
                    break;
                }
            }
            if (users != null)
            {
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ParameterCode.MatchState, RoomState.Matching);
                EventData ed = new EventData((byte)(EventCode.Matching));
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
                Server.log.Info("匹配进行中，请稍后！。。。。。。");
                if (Server.Instance.queueMaching.Count > 0)
                {
                    Room tempRoom = Server.Instance.queueMaching.Dequeue();
                    MatchingBattle(tempRoom, sendParameters);
                }
            }
        }

        private void MatchingBattle(Room room, SendParameters sendParameters)
        {
            bool isEnterBigRoom = false;
            if (Server.Instance.bigRooms.Count < 1)//没有大房间时创建一个大房间
            {
                BigRoom bigRoom = new BigRoom();
                Guid bigRoomID = Guid.NewGuid();
                bigRoom.BigRoomID = bigRoomID;
                bigRoom.isHaveRefused = false;
                bigRoom.acceptCount = 0;
                bigRoom.ATeam = new List<User>();
                bigRoom.BTeam = new List<User>();
                Server.Instance.bigRooms.Add(bigRoomID, bigRoom);
                bigRoom.ATeam.AddRange(room.Room_players);
                isEnterBigRoom = true;
            }
            else
            {
                foreach (BigRoom bigRoom in Server.Instance.bigRooms.Values)//遍历每个大房间
                {
                    if (bigRoom.ATeam.Count <= bigRoom.BTeam.Count)//如果当前A队的人员<=B队的人员，则以A队为主
                    {
                        if ((5 - bigRoom.ATeam.Count) >= room.Room_players.Count)//如果当前A队的人员还未满员，且当前小房间的人数<=未满的人员时
                        {
                            bigRoom.ATeam.AddRange(room.Room_players);
                            isEnterBigRoom = true;
                            if ((bigRoom.ATeam.Count + bigRoom.BTeam.Count) == 10)//通知这个十个人已经匹配到
                            {
                                byte[] bigRoomBytes = Tool.SerializeByProtobufNet<BigRoom>(bigRoom);
                                Dictionary<byte, object> data = new Dictionary<byte, object>();
                                data.Add((byte)ParameterCode.MatchState, RoomState.Matched);
                                data.Add((byte)ParameterCode.BigRoom, bigRoomBytes);
                                EventData ed = new EventData((byte)EventCode.Matched, data);
                                foreach (User item in bigRoom.ATeam)
                                {
                                    foreach (Peer peer in Server.Instance.listPeer)
                                    {
                                        if (peer.user.Id == item.Id)
                                        {
                                            peer.SendEvent(ed, sendParameters);
                                        }
                                    }
                                }
                                foreach (User item in bigRoom.BTeam)
                                {
                                    foreach (Peer peer in Server.Instance.listPeer)
                                    {
                                        if (peer.user.Id == item.Id)
                                        {
                                            peer.SendEvent(ed, sendParameters);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                    else//如果当前A队的人员>B队的人员，则以B队为主
                    {
                        if ((5 - bigRoom.BTeam.Count) >= room.Room_players.Count)//如果当前b队的人员还未满员，且当前小房间的人数<=未满的人员时
                        {
                            bigRoom.BTeam.AddRange(room.Room_players);
                            isEnterBigRoom = true;
                            if ((bigRoom.ATeam.Count + bigRoom.BTeam.Count) == 10)//通知这个十个人
                            {
                                byte[] bigRoomBytes = Tool.SerializeByProtobufNet(bigRoom);
                                Dictionary<byte, object> data = new Dictionary<byte, object>();
                                data.Add((byte)ParameterCode.MatchState, RoomState.Matched);
                                data.Add((byte)ParameterCode.BigRoom, bigRoomBytes);
                                EventData ed = new EventData((byte)EventCode.Matched, data);
                                foreach (User item in bigRoom.ATeam)
                                {
                                    foreach (Peer peer in Server.Instance.listPeer)
                                    {
                                        if (peer.user.Id == item.Id)
                                        {
                                            peer.SendEvent(ed, sendParameters);
                                        }
                                    }
                                }
                                foreach (User item in bigRoom.BTeam)
                                {
                                    foreach (Peer peer in Server.Instance.listPeer)
                                    {
                                        if (peer.user.Id == item.Id)
                                        {
                                            peer.SendEvent(ed, sendParameters);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
            if (!isEnterBigRoom)//没有满足的大房间时，再创建一个大房间
            {
                BigRoom bigRoom = new BigRoom();
                Guid bigRoomID = Guid.NewGuid();
                bigRoom.isHaveRefused = false;
                bigRoom.acceptCount = 0;
                bigRoom.BigRoomID = bigRoomID;
                bigRoom.ATeam = new List<User>();
                bigRoom.BTeam = new List<User>();
                Server.Instance.bigRooms.Add(bigRoomID, bigRoom);
                bigRoom.ATeam.AddRange(room.Room_players);
            }
        }
    }
}
