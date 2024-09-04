using UnityEngine;
using UnityEngine.UI;

public class Adapter_UI : MonoBehaviour
{
    public float referenceResolutionX = 720f;//设置分辨率
    public float referenceResolutionY = 1280f;
    public float referenceResolutionAspectRatio;
    public float currentDeviceResolutionAspectRatio;//1440/720=2  2960/1280= 2.3125
    public CanvasScaler canvasScaler;

    //public Text text_Show;//Test

    void Awake()
    {
        referenceResolutionAspectRatio = referenceResolutionX / referenceResolutionY;//720/1280=0.5625

        currentDeviceResolutionAspectRatio = 1f * Screen.width / Screen.height;//1440/2960 = 0.4864865

        if (currentDeviceResolutionAspectRatio > referenceResolutionAspectRatio)//UI适配宽度
        {
            canvasScaler.matchWidthOrHeight = 0f;
        }
        else if (currentDeviceResolutionAspectRatio < referenceResolutionAspectRatio)//UI适配高度
        {
            canvasScaler.matchWidthOrHeight = 1f;
        }
        else
        {
            canvasScaler.matchWidthOrHeight = 0.5f;
        }
    }
    void Start()
    {
        //text_Show.text = Screen.width + "   " + Screen.height + "\ncurrentDeviceResolutionAspectRatio：" + currentDeviceResolutionAspectRatio +
        //    "\nreferenceResolutionAspectRatio：" + referenceResolutionAspectRatio +
        //    "\ncanvasScaler.matchWidthOrHeight：" + canvasScaler.matchWidthOrHeight +
        //    "\nCanvas.Scale：" + canvasScaler.transform.localScale;
    }
}