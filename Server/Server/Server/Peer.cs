using FTools.Codes;
using FTools.Community;
using FTools.Tools;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using Server.Handler;

namespace Server
{
    public class Peer : ClientPeer
    {
        public User user;
        public Room room;

        public Peer(InitRequest initRequest) : base(initRequest)
        {

        }

        /// <summary>
        /// 客户端断开连接时的后续操作
        /// </summary>
        /// <param name="reasonCode"></param>
        /// <param name="reasonDetail"></param>
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            Server.log.Info("【客户端】已断开连接！" + reasonCode.ToString());
            Server.Instance.listPeer.Remove(this);//将其移除
        }

        /// <summary>
        /// 处理客户端发送来的请求
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            BaseHandler handler = Tool.GetValue(Server.Instance.dictBaseHandler, (OperationCode)operationRequest.OperationCode);
            if (handler != null)
            {
                handler.OnOperationRequest(operationRequest, sendParameters, this);
            }
            else
            {
                handler = Tool.GetValue(Server.Instance.dictBaseHandler, OperationCode.Nil);
                handler.OnOperationRequest(operationRequest, sendParameters, this);
            }
        }
    }
}
