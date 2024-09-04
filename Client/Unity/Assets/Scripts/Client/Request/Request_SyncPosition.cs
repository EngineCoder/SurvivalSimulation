using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
using FTools;
using System.Collections.Generic;

public class Request_SyncPosition : BaseRequest
{
    [HideInInspector]
    public Vector3 position;

    public Request_SyncPosition()
    {
        operationCode = Code_Operation.SyncPosition;
    }

    public override void OperationRequest()
    {
        Dictionary<byte, object> data = new Dictionary<byte, object>();

        data.Add((byte)Code_Parameter.X, position.x);
        data.Add((byte)Code_Parameter.Y, position.y);
        data.Add((byte)Code_Parameter.Z, position.z);

        Client.Peer.SendOperation((byte)operationCode, data, SendOptions.SendReliable);
    }

    public override void OnOperationResponse(OperationResponse operationResponse)
    {
        //throw new System.NotImplementedException();
    }
}
