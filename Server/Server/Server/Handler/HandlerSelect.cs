using FTools.Codes;
using FTools.Community;
using FTools.Tools;
using Photon.SocketServer;
using System.Collections.Generic;

namespace Server.Handler
{
    public class HandlerSelect : BaseHandler
    {
        public HandlerSelect()
        {
            operationCode = OperationCode.SelectPlayer;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, Peer peer)
        {
            //1、接收客户端发送来的数据
            string nickname = Tool.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.Nickname) as string;

            //2、进行数据处理
            User user = null;
            foreach (Peer item in Server.Instance.listPeer)
            {
                if (item.user.User_nickname == nickname)
                {
                    user = item.user;
                    break;
                }
            }

            //3、创建一个响应，反馈给客户端
            OperationResponse respone = new OperationResponse(operationRequest.OperationCode);
            if (user != null)
            {
                byte[] bytes = Tool.SerializeByProtobufNet(user);
                Dictionary<byte, object> kv = new Dictionary<byte, object>();
                kv.Add((byte)ParameterCode.Nickname, bytes);
                respone.Parameters = kv;
                respone.ReturnCode = 0;
            }
            else
            {
                respone.ReturnCode = 1;
            }
            //向客户端发送反馈信息
            peer.SendOperationResponse(respone, sendParameters);
        }
    }
}
