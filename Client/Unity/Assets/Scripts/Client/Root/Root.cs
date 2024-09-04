using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using STools.Tool_Manager;


public class Root : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Manager_ObjectPool.Instance.Initial();
        Manager_Audio.Instance.PlayBGMAudioByMusicType((ResType_Music.BGM)Random.Range(0, 4), Camera.main.transform.position);
        Manager_Panel.Instance.PushStack_Panel(Type_Panel.Login);
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {

        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Manager_Tip.Instance.Update_TipContent(1, "错误提示", "用户名或密码错误");
            Manager_Panel.Instance.PushStack_Panel(Type_Panel.Tip);
        }
    }
}
