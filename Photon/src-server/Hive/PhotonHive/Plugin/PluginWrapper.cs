using System;
using System.Collections.Generic;

using ExitGames.Logging;

using Newtonsoft.Json;

using Photon.SocketServer.Diagnostics;

namespace Photon.Hive.Plugin
{
    class PluginWrapper : IGamePlugin
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private static readonly LogCountGuard logCountGuard = new LogCountGuard(new TimeSpan(0, 0, 1));
        private static readonly LogCountGuard logCountGuard2 = new LogCountGuard(new TimeSpan(0, 0, 1));

        private static readonly JsonSerializerSettings serializeSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MaxDepth = 1,
            PreserveReferencesHandling = PreserveReferencesHandling.None
        };

        #region Constructor and Destructor

        public PluginWrapper(IPluginInstance instance)
        {
            this.PluginInstance = instance;
            this.ErrorPlugin = instance.Plugin as ErrorPlugin;
        }
        #endregion

        #region Properties

        public IGamePlugin Plugin => this.PluginInstance.Plugin;

        public EnvironmentVersion EnvironmentVersion => this.PluginInstance.Version;

        public IPluginInstance PluginInstance { get; }

        public ErrorPlugin ErrorPlugin { get; }

        #endregion

        #region Methods
        private void StrictModeCheck(ICallInfo callInfo)
        {
            if (!callInfo.StrictModeCheck(out var errorMsg))
            {
                var infoTypeName = callInfo.GetType().ToString();
                ((IGamePlugin)this).ReportError(ErrorCodes.MissingCallProcessing, null, infoTypeName);
                callInfo.Fail(errorMsg);
            }
        }

        private static void CallFailSafe(ICallInfo info, string errorMessage)
        {
            if (!info.IsProcessed)
            {
                info.Fail(errorMessage);
            }
        }

        private void ExceptionHandler(ICallInfo info, Exception exception)
        {
            Log.Error(logCountGuard,
                $"Exception during plugin call. OpCode={info.OperationRequest.OperationCode}, " +
                              $"Parameters={JsonConvert.SerializeObject(info.OperationRequest.Parameters, serializeSettings)}, " +
                              $"WebFlags={info.OperationRequest.WebFlags}," +
                              $" p={((CallInfo)info).Peer}",
                 exception);
            try
            {
                this.Plugin.ReportError(ErrorCodes.UnhandledException, exception);
            }
            catch (Exception e)
            {
                Log.Error(logCountGuard2,
                         $"Exception ReportError call. OpCode={info.OperationRequest.OperationCode}, " +
                          $"Parameters={JsonConvert.SerializeObject(info.OperationRequest.Parameters, serializeSettings)}, " +
                          $"WebFlags={info.OperationRequest.WebFlags}," +
                          $" p={((CallInfo)info).Peer}",
                        e);
            }
            finally
            {
                CallFailSafe(info, exception.ToString());
            }
        }

        #endregion

        #region Interface Implementations

        public string Name => this.PluginInstance.Plugin.Name;
        public string Version => this.PluginInstance.Plugin.Version;

        public bool IsPersistent
        {
            get
            {
                var isPersistent = false;
                try
                {
                    isPersistent = this.Plugin.IsPersistent;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                return isPersistent;
            }
        }

        public void BeforeCloseGame(IBeforeCloseGameCallInfo info)
        {
            try
            {
                this.Plugin.BeforeCloseGame(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                this.ExceptionHandler(info, e);
            }
        }

        public void BeforeJoin(IBeforeJoinGameCallInfo info)
        {
            try
            {
                this.Plugin.BeforeJoin(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                this.ExceptionHandler(info, e);
            }
        }

        public void BeforeSetProperties(IBeforeSetPropertiesCallInfo info)
        {
            try
            {
                this.Plugin.BeforeSetProperties(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                this.ExceptionHandler(info, e);
            }
        }

        public void OnCloseGame(ICloseGameCallInfo info)
        {
            try
            {
                this.Plugin.OnCloseGame(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                this.ExceptionHandler(info, e);
            }
        }

        public void OnCreateGame(ICreateGameCallInfo info)
        {
            try
            {
                this.Plugin.OnCreateGame(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                this.ExceptionHandler(info, e);
            }
        }

        public void OnJoin(IJoinGameCallInfo info)
        {
            try
            {
                this.Plugin.OnJoin(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                this.ExceptionHandler(info, e);
            }
        }

        public void OnLeave(ILeaveGameCallInfo info)
        {
            try
            {
                this.Plugin.OnLeave(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                this.ExceptionHandler(info, e);
            }
        }

        public void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            try
            {
                this.Plugin.OnRaiseEvent(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                this.ExceptionHandler(info, e);
            }
        }

        public void OnSetProperties(ISetPropertiesCallInfo info)
        {
            try
            {
                this.Plugin.OnSetProperties(info);
                this.StrictModeCheck(info);
            }
            catch (Exception e)
            {
                this.ExceptionHandler(info, e);
            }
        }

        public bool OnUnknownType(Type type, ref object value)
        {
            try
            {
                return this.Plugin.OnUnknownType(type, ref value);
            }
            catch (Exception e)
            {
                Log.Error(e);
                this.Plugin.ReportError(ErrorCodes.UnhandledException, e);
                return false;
            }
        }

        public bool SetupInstance(IPluginHost host, Dictionary<string, string> config, out string errorMsg)
        {
            try
            {
                return this.Plugin.SetupInstance(host, config, out errorMsg);
            }
            catch (Exception e)
            {
                errorMsg = e.Message;
                Log.Error(e);
                return false;
            }
        }

        public void ReportError(short errorCode, Exception e, object state = null)
        {
            try
            {
                this.Plugin.ReportError(errorCode, e, state);
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
        }
        #endregion
    }
}
