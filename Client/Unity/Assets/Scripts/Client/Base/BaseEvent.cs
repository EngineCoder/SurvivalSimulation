using UnityEngine;
using System.Collections;
using FTools;
using ExitGames.Client.Photon;

public abstract class BaseEvent : MonoBehaviour {

    public Code_Event eventCode;

    public abstract void OnEvent(EventData eventData);

    public virtual void Start()
    {
        Client.Instance.AddEvent(this);
    }

    public virtual void OnDestory()
    {
        Client.Instance.RemoveEvent(this);
    }
}
