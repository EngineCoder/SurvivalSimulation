using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blood : MonoBehaviour
{
    public Image img_red;
    public Image img_yellow;

    public float hp = 100;
    private bool isdasd = false;
    public float target = 0;
    public float speed = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hp -= 90;
            img_red.fillAmount = hp / 100f;
            isdasd = true;
        }

        if (isdasd)
        {
            img_yellow.fillAmount = Mathf.SmoothStep(img_yellow.fillAmount, img_red.fillAmount, Time.unscaledDeltaTime*27);
        }
    }
}
