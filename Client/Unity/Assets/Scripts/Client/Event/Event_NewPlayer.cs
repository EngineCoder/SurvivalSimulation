using FTools;
using FTools.Tools;
using ExitGames.Client.Photon;

public class Event_NewPlayer : BaseEvent
{
    private PlayerCC player;

    public Event_NewPlayer()
    {
        eventCode = Code_Event.NewPlayer;
    }

    public override void Start()
    {
        base.Start();
        player = GetComponent<PlayerCC>();
    }

    public override void OnEvent(EventData eventData)
    {
        string userid = Tool_Dict.GetValue<byte, object>(eventData.Parameters, (byte)Code_Parameter.UserID) as string;
        player.OnNewPlayerEvent(userid);
    }
}
