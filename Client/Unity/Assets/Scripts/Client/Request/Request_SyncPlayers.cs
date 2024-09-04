using ExitGames.Client.Photon;
using FTools.Tools;
using FTools;
using System.Collections.Generic;
using UnityEngine;

using System.Xml.Serialization;
using System.IO;

class Request_SyncPlayers : BaseRequest
{
    private PlayerCC player;

    public Request_SyncPlayers()
    {
        operationCode = Code_Operation.SyncPlayers;
    }

    public override void Start()
    {
        base.Start();
        player = GetComponent<PlayerCC>();
    }

    public override void OperationRequest()
    {
        Client.Peer.SendOperation((byte)operationCode, null, SendOptions.SendReliable);
    }

    public override void OnOperationResponse(OperationResponse operationResponse)
    {

        string list_UsernameXmlString = Tool_Dict.GetValue<byte, object>(operationResponse.Parameters, (byte)Code_Parameter.UserIdList) as string;

        using (StringReader sr = new StringReader(list_UsernameXmlString))
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<string>));
            List<string> list_UserId = (List<string>)xmlSerializer.Deserialize(sr);
            player.OnSyncPlayerResponse(list_UserId);
        }
    }
}

