using UnityEngine;
using System.Collections;

public class Controller_MainCamera : MonoBehaviour
{
    GameObject cameraTarget;//摄像机看向的目标
    public float rotateSpeed;//旋转速度

    public float offsetDistance;//距离偏移
    public float offsetHeight;//高度偏移
    public float smoothing;//平滑

    float rotate;
    Vector3 offset;
    Vector3 lastPosition;

    float percentage = 0f;

    void Start()
    {
        cameraTarget = GameObject.FindGameObjectWithTag("Player");

        lastPosition = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y + offsetHeight, cameraTarget.transform.position.z - offsetDistance);

        offset = new Vector3(0, offsetHeight, -offsetDistance);

        percentage = offsetHeight / offsetDistance;
    }

    void Update()
    {
        //是否取消跟踪角色
        if (Input.GetKeyDown(KeyCode.Q))
        {
            rotate = -45;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            rotate = 45;
        }
        else
        {
            rotate = 0;
        }

        //鼠标滑动滚轮，控制摄像机的拉近和拉运。我想实现缓动效果怎么实现？
        if (Input.mouseScrollDelta.y != 0)
        {
            offsetHeight += Input.mouseScrollDelta.y;

            if (offsetHeight<6)
            {
                offsetHeight = 6f;
            }
            else if (offsetHeight>20)
            {
                offsetHeight = 20f;
            }

            offsetDistance = offsetHeight / percentage;
            offset = new Vector3(0, offsetHeight, -offsetDistance);
        }
    }

    void LateUpdate()
    {

        offset = Quaternion.AngleAxis(rotate * rotateSpeed, Vector3.up) * offset;

        transform.position = new Vector3(
            Mathf.Lerp(lastPosition.x, cameraTarget.transform.position.x + offset.x, smoothing * Time.deltaTime),
            Mathf.Lerp(lastPosition.y, cameraTarget.transform.position.y + offset.y, smoothing * Time.deltaTime),
            Mathf.Lerp(lastPosition.z, cameraTarget.transform.position.z + offset.z, smoothing * Time.deltaTime));

        //看向物体
        transform.LookAt(cameraTarget.transform.position);

        //更新最终的位置
        lastPosition = transform.position;
    }
}