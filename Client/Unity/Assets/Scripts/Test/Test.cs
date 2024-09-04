using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
//using FTools;
public class Test : MonoBehaviour
{
    public Button btn;
    public Ease ease = Ease.Linear;
    public bool isFirst = false;
    // Use this for initialization
    void Start()
    {
        btn.onClick.AddListener(() =>
        {
            isFirst = !isFirst;
            if (isFirst)
            {
                Tween tween = (transform as RectTransform).DOAnchorPos3D(new Vector3(300, 300, 0), 0.6f);
                tween.SetEase(ease);
            }
            else
            {
                Tween tween = (transform as RectTransform).DOAnchorPos3D(new Vector3(-300, -300, 0), 0.6f);
                tween.SetEase(ease);
            }
            
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
