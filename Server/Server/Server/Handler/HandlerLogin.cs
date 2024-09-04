using FTools;
using FTools.Utils;
using Photon.SocketServer;
using Server.Manager;
using System;
using System.Collections.Generic;
using FTools.Codes;
using Server.Interface;
using FTools.Community;

namespace Server.Handler
{
    public class HandlerLogin : BaseHandler
    {
        public HandlerLogin()
        {
            operationCode = OperationCode.Login;
        }

        /// <summary>
        /// 处理客户端发送来的登录请求数据
        /// </summary>
        /// <param name="operationRequest"></param>
        /// <param name="sendParameters"></param>
        /// <param name="peer"></param>
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, Peer peer)
        {
            //1、接收客户端发送来的数据
            string account = Tool.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.Account) as string;
            string password = Tool.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.Password) as string;

            //2、进行数据处理
            IOperationTable<User> userManager = new UserTable();
            User user = userManager.SelectByColumnName1AndColumnName2("User_account", "User_password", account, password);

            //3、创建一个响应，反馈给客户端
            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            if (user != null)
            {
                byte[] bytes_user = Tool.SerializeByProtobufNet(user);
                Dictionary<byte, object> keyValues = new Dictionary<byte, object>();
                keyValues.Add((byte)ParameterCode.User, bytes_user);
                keyValues.Add((byte)ReturnCode.Success, "正在进入游戏！请稍后。。。");
                response.Parameters = keyValues;
                response.ReturnCode = (short)ReturnCode.Success;
                peer.user = user;
                user.User_lastLogin_date = DateTime.UtcNow;
                userManager.Update(user);
            }
            else
            {
                user = userManager.SelectByColumnName("User_account", account);
                if (user != null)
                {
                    Dictionary<byte, object> keyValues = new Dictionary<byte, object>();
                    keyValues.Add((byte)ReturnCode.Failed, "密码输入错误，请重新输入！");
                    response.Parameters = keyValues;
                    response.ReturnCode = (short)ReturnCode.Failed;
                }
                else
                {
                    Dictionary<byte, object> keyValues = new Dictionary<byte, object>();
                    keyValues.Add((byte)ReturnCode.Failed, "账号不存在，请重新输入！");
                    response.Parameters = keyValues;
                    response.ReturnCode = (short)ReturnCode.Failed;
                }
            }
            //向客户端发送反馈信息
            peer.SendOperationResponse(response, sendParameters);
        }
    }
}
