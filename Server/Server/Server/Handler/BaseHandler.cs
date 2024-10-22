﻿using FTools.Codes;
using Photon.SocketServer;
using Server.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler
{
    public abstract class BaseHandler
    {
        public OperationCode operationCode;
        public abstract void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, Peer peer);
    }
}
