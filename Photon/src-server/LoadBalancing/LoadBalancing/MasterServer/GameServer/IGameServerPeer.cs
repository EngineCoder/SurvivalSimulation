using System;
using ExitGames.Concurrency.Fibers;
using Photon.SocketServer;

namespace Photon.LoadBalancing.MasterServer.GameServer
{
    public interface IGameServerPeer
    {
        IRpcProtocol Protocol { get; }

        IFiber RequestFiber { get; }

        string RemoteIP { get; }

        int RemotePort { get; }

        void AttachToContext(GameServerContext context);

        void DetachFromContext();

        void Disconnect(int disconnectError);

        void WaitForDisconnect(int timeout);
    }
}
