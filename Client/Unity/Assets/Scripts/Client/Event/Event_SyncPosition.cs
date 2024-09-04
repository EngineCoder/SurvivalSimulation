using ExitGames.Client.Photon;
using FTools;
using FTools.Tools;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

public class Event_SyncPosition : BaseEvent
{
    private PlayerCC player;

    public Event_SyncPosition()
    {
        eventCode = Code_Event.SyncPosition;
    }

    public override void Start()
    {
        base.Start();
        player = GetComponent<PlayerCC>();
    }


    public override void OnEvent(EventData eventData)
    {
        string playerDataListString = (string)Tool_Dict.GetValue<byte, object>(eventData.Parameters, (byte)Code_Parameter.PlayerDataList);

        using (StringReader sr = new StringReader(playerDataListString))
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<PlayerData>));
            List<PlayerData> playerDatas = (List<PlayerData>)xmlSerializer.Deserialize(sr);
            player.OnSyncPositionEvent(playerDatas);
        }
    }
}
