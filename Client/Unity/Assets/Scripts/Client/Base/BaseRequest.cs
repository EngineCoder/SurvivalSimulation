using FTools;
using UnityEngine;

using ExitGames.Client.Photon;

public abstract class BaseRequest : SingleMode<BaseRequest>
{
    public Code_Operation operationCode;

    /// <summary>
    /// BaseRequest
    /// </summary>
    public abstract void OperationRequest();


    /// <summary>
    /// Response
    /// </summary>
    public abstract void OnOperationResponse(OperationResponse operationResponse);


    public virtual void Start()
    {
        Client.Instance.AddRequest(this);
    }


    public override void OnDestroy()
    {
        base.OnDestroy();

        Client.Instance.RemoveRequest(this);
    }
}
