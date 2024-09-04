using Photon.Hive.Diagnostics;
using Photon.SocketServer.Diagnostics;

using System;

namespace Photon.LoadBalancing.GameServer
{
    public interface IGSLogContext
    {
        LogCountGuard FullGameReinitLogGuard { get; }
        LogCountGuard InactivePeerLogGuard { get; }
    }

    public class GSLogContext : HiveLogContext, IGSLogContext, IGameClientPeerLogContext
    {
        public new static readonly GSLogContext Default = new GSLogContext();

        public LogCountGuard FullGameReinitLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 1, 0));
        public LogCountGuard InactivePeerLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 10));

        // from GameClientPeer
        public LogCountGuard SecureConnectionLogGuard { get; } = new LogCountGuard(new TimeSpan(0, 0, 30));

    }
}
