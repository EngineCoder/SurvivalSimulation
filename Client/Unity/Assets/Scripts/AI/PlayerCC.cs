using FTools;
using FTools.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCC : MonoBehaviour
{
    #region 移动，跳跃，自由落体
    //移动
    public float moveSpeed = 2f;
    public float turnSpeed = 3;//转向速度
    private float x;
    private float z;

    public CharacterController cc;

    //自由落体
    public float gravity = -9.81f;
    public Vector3 velocity = Vector3.zero;
    public Transform groundCheck;
    public float groundCheckRadius = 0.25f;
    public LayerMask groundMask;
    public bool isGrounded = true;

    //跳跃
    public float jumpHeight = 2f;
    #endregion


    #region Player Data
    public string userid;
    public bool isLocalPalyer = true;

    public GameObject prefab_Player;
    public GameObject player;


    private Vector3 lastPosition = Vector3.zero;


    private Request_SyncPosition request_SyncPosition;
    private Request_SyncPlayers request_SyncPlayer;//创建其他玩家


    /// <summary>
    /// 保存其他角色
    /// </summary>
    public Dictionary<string, GameObject> dict_OtherPlayerGameObject = new Dictionary<string, GameObject>();
    #endregion



    #region Movement
    [HideInInspector] float angle;
    [HideInInspector] Quaternion targetRotation;//旋转目标
    [HideInInspector] Transform transform_Camera;
    #endregion


    void Start()
    {
        transform_Camera = Camera.main.transform;
        cc = player.GetComponent<CharacterController>();

        player.GetComponent<Renderer>().material.color = Color.red;

        request_SyncPosition = GetComponent<Request_SyncPosition>();

        request_SyncPlayer = GetComponent<Request_SyncPlayers>();

        request_SyncPlayer.OperationRequest();//该请求不需要传递任何参数

        InvokeRepeating("SyncPosition", 1, 0.02f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPalyer)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                float tempY = velocity.y;
                velocity.y = tempY;
            }

            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");

            //跳跃
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            //自由落体
            velocity.y += gravity * Time.deltaTime;
            cc.Move(velocity * Time.deltaTime * 0.5f);

            //移动输入控制运算
            if (Mathf.Abs(x) < 0.2f && Mathf.Abs(z) < 0.2f)
            {
                return;
            }

            //更新朝向
            CalculateDirectionAndRotateAndMove();
        }
    }

    /// <summary>
    /// 朝向，旋转，移动
    /// </summary>
    void CalculateDirectionAndRotateAndMove()
    {
        //计算朝向
        angle = Mathf.Atan2(x, z);//键盘输入，W,A,S,D 返回一个弧度
        angle = Mathf.Rad2Deg * angle;//弧度转角度
        angle += transform_Camera.eulerAngles.y;

        //获得旋转角度的目标，角度转四元数
        targetRotation = Quaternion.Euler(0, angle, 0);

        //进行旋转
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        //移动
        cc.Move(player.transform.forward * Time.deltaTime * moveSpeed);
    }

    /// <summary>
    /// 位置改变，则向服务器发送请求
    /// </summary>
    private void SyncPosition()
    {
        if (Vector3.Distance(player.transform.position, lastPosition) > 0.01f)
        {
            lastPosition = player.transform.position;
            request_SyncPosition.position = player.transform.position;
            request_SyncPosition.OperationRequest();
        }
    }


    public void OnSyncPlayerResponse(List<string> list_UserId)
    {
        //创建其他客户端的Player角色
        foreach (string userid in list_UserId)
        {
            OnNewPlayerEvent(userid);
        }
    }

    /// <summary>
    /// 当有新玩家进来时
    /// </summary>
    /// <param name="username"></param>
    public void OnNewPlayerEvent(string userid)
    {
        GameObject go_Player = (GameObject)Instantiate(prefab_Player);
        dict_OtherPlayerGameObject.Add(userid, go_Player);
        go_Player.name = userid;
    }


    /// <summary>
    /// 接收服务端发来的各玩家信息
    /// </summary>
    /// <param name="playerDatas"></param>
    public void OnSyncPositionEvent(List<PlayerData> playerDatas)
    {
        foreach (PlayerData pd in playerDatas)
        {
            GameObject go = Tool_Dict.GetValue<string, GameObject>(dict_OtherPlayerGameObject, pd.userid);

            if (go != null)//表示不更新自身的位置
            {
                go.transform.position = new Vector3() { x = pd.pos.x, y = pd.pos.y, z = pd.pos.z };
            }
        }
    }

}
