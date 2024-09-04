using System;
using System.Collections.Generic;
using System.IO;
using FTools.Codes;
using FTools.Community;
using Photon.SocketServer;
using Server.Handler;
using ExitGames.Logging;
using Microsoft.Extensions.Configuration;
using ExitGames.Logging.Log4Net;
using log4net.Config;

namespace Server
{
    public class Server : ApplicationBase
    {
        public static new Server Instance { get; private set; }

        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 请求处理字典：请求码-具体处理
        /// <para>1）存储所有 请求码-处理器 列表</para>
        /// </summary>
        public Dictionary<OperationCode, BaseHandler> dictBaseHandler = new Dictionary<OperationCode, BaseHandler>();

        /// <summary>
        /// 存储所有的客户端peer
        /// </summary>
        public List<Peer> listPeer = new List<Peer>();//通过这个列表获得所有的客户端peer

        /// <summary>
        /// 存储所有的房间信息
        /// </summary>
        public List<Room> listRooms = new List<Room>();//存储所有的房间信息

        #region 匹配系统
        public Queue<Room> queueMaching = new Queue<Room>();//匹配队列
        public Dictionary<Guid, BigRoom> bigRooms = new Dictionary<Guid, BigRoom>();//大房间

        public Server(IConfiguration configuration) : base(configuration)
        {
            Instance = this;
        }

        #endregion


        /// <summary>
        /// 当有客户端连接时，创建一个Peer,来实现客户端与服务端的通信
        /// </summary>
        /// <param name="initRequest"></param>
        /// <returns></returns>
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            Peer peer = new Peer(initRequest);
            listPeer.Add(peer);
            log.Info("【客户端】已连接！" + peer.RemoteIPAddress);
            return peer;
        }

        /// <summary>
        /// 服务端启动之后的初始化
        /// </summary>
        protected override void Setup()
        {
            Instance = this;

            #region 日志的初始化
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(Path.Combine(this.ApplicationRootPath, "FServer"), "log");//设置日志的输出目录
            FileInfo configFileInfo = new FileInfo(Path.Combine(this.BinaryPath, "log4net.config"));//读取日志配置文件
            if (configFileInfo.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);//设置使用的是哪个日志插件 Log4Net
                XmlConfigurator.ConfigureAndWatch(configFileInfo);//使用log4Net这个插件配置日志文件
            }
            log.Info("【服务器】已启动!");
            InitHandler();
            #endregion
        }

        /// <summary>
        /// 初始化【请求-处理】逻辑
        /// </summary>
        void InitHandler()
        {
            HandlerLogin handler_Login = new HandlerLogin();//登录
            dictBaseHandler.Add(handler_Login.operationCode, handler_Login);

            HandlerCreateRoom handler_CreateRoom = new HandlerCreateRoom();//创建房间
            dictBaseHandler.Add(handler_CreateRoom.operationCode, handler_CreateRoom);

            HandlerSelect handler_Select = new HandlerSelect();//查询玩家
            dictBaseHandler.Add(handler_Select.operationCode, handler_Select);

            HandlerInvite handler_Invite = new HandlerInvite();//发送邀请
            dictBaseHandler.Add(handler_Invite.operationCode, handler_Invite);

            HandlerAcceptInvite handler_AcceptInvite = new HandlerAcceptInvite();//接受邀请
            dictBaseHandler.Add(handler_AcceptInvite.operationCode, handler_AcceptInvite);

            HandlerFindMatch handlerFindMatch = new HandlerFindMatch();//寻找匹配
            dictBaseHandler.Add(handlerFindMatch.operationCode, handlerFindMatch);

            HandlerRefuseMatching handlerMatchingRefuse = new HandlerRefuseMatching();//匹配中拒绝
            dictBaseHandler.Add(handlerMatchingRefuse.operationCode, handlerMatchingRefuse);

            HandlerExitRoom handlerExitRoom = new HandlerExitRoom();//玩家离开房间
            dictBaseHandler.Add(handlerExitRoom.operationCode, handlerExitRoom);

            HandlerAcceptMatched handlerAcceptMatched = new HandlerAcceptMatched();//接受匹配
            dictBaseHandler.Add(handlerAcceptMatched.operationCode, handlerAcceptMatched);

            HandlerRefuseMatched handlerRefuseMatched = new HandlerRefuseMatched();//拒绝匹配
            dictBaseHandler.Add(handlerRefuseMatched.operationCode, handlerRefuseMatched);
        }

        /// <summary>
        /// 服务端关闭的时候
        /// </summary>
        protected override void TearDown()
        {
            log.Info("【服务器】已关闭!");
            dictBaseHandler.Clear();
            listPeer.Clear();
            listRooms.Clear();
        }
    }
}
