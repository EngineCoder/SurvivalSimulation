using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //(transform as RectTransform).localEulerAngles = -(transform.parent as RectTransform).localEulerAngles;

        transform.Rotate(new Vector3(0,0,10), Space.Self);
    }
}
