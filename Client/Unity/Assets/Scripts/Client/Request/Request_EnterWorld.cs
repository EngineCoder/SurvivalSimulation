using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Request_EnterWorld : BaseRequest
{

    public override void OperationRequest()
    {
        Client.Peer.OpCustom((byte)FTools.Code_Operation.EnterWorld, null, true);
    }

    public override void OnOperationResponse(OperationResponse operationResponse)
    {
        
    }
}
