using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STools.Tool_Manager;
public class Loading_Scene : MonoBehaviour
{
    UnityEngine.UI.Image loadImage;
    public int levelIndex=2;
    public Request_EnterWorld request_EnterWorld;

    private void Awake()
    {
        loadImage = transform.Find("Img_FillBG/Img_Fill").GetComponentInChildren<UnityEngine.UI.Image>();
    }

    private void Start()
    {
        request_EnterWorld = GetComponent<Request_EnterWorld>();

        Load(levelIndex);
    }

    public void Load(int levelIndex)
    {
        Manager_Scene.Instance.StartCoroutine(Manager_Scene.Instance.LoadScene(levelIndex, SetFillAmount));
    }

    private void SetFillAmount(float fillAmount)
    {
        loadImage.fillAmount = fillAmount;
        request_EnterWorld.OperationRequest();

    }
}
