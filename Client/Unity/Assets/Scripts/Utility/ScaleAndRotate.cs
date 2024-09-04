using UnityEngine;
using System.Collections;


public class ScaleAndRotate : MonoBehaviour 
{
    private Touch oldTouch1;  //上次触摸点1(手指1)
    private Touch oldTouch2;  //上次触摸点2(手指2)

    void Update()
    {
        //没有触摸，就是触摸点为0
       
        Touch newTouch1 = Input.GetTouch(0);
        Touch newTouch2 = Input.GetTouch(1);

        //第2点刚开始接触屏幕, 只记录，不做处理
        if (newTouch2.phase == TouchPhase.Began)
        {
            oldTouch2 = newTouch2;
            oldTouch1 = newTouch1;
            return;
        }

        //计算老的两点距离和新的两点间距离，变大要放大模型，变小要缩放模型
        float oldDistance = Vector2.Distance(oldTouch1.position, oldTouch2.position);
        float newDistance = Vector2.Distance(newTouch1.position, newTouch2.position);

        //两个距离之差，为正表示放大手势， 为负表示缩小手势
        float offset = newDistance - oldDistance;

        //放大因子， 一个像素按 0.01倍来算(100可调整)
        float scaleFactor = offset / 5000f;
        Vector3 tempScale = transform.localScale;
        Vector3 newScale = new Vector3(tempScale.x + scaleFactor,
                                    tempScale.y + scaleFactor,
                                    tempScale.z + scaleFactor);
        //在什么情况下进行缩放
        if (newScale.x >= 0.2f && newScale.y >=0.2f && newScale.z >= 0.2f)
        {
            transform.localScale = newScale;
        }

        //记住最新的触摸点，下次使用
        oldTouch1 = newTouch1;
        oldTouch2 = newTouch2;
    }
}