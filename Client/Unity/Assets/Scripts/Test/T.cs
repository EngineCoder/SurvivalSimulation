using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class T : MonoBehaviour
{
    public Button button;

    //void Update()
    //{
    //    //button.transform.DORotate(new Vector3(0,0,-18), 4, RotateMode.LocalAxisAdd).Complete();
    //    //button.transform.DORotate(new Vector3(0, 0, -18), 4, RotateMode.WorldAxisAdd);
    //    DoLeft();
    //    //button1.transform.DORotate(new Vector3(0, 0, -18), 4, RotateMode.Fast).Complete();
    //}

    //public void DoRight()
    //{
    //    button.transform.DORotate(new Vector3(0, 0, 18), 2, RotateMode.Fast).OnComplete(DoLeft);
    //}
    //public void DoLeft()
    //{
    //    button.transform.DORotate(new Vector3(0, 0, -18), 2, RotateMode.Fast).OnComplete(DoRight);
    //}


    public float byValue = 10;

    public float duratation = 0.3f;

    //public RotateMode rotateModel = RotateMode.Fast;

    public bool rotate = true;

    //public Ease ease = Ease.OutSine;

    public int MaxCount = 4;

    private void OnEnable()
    {
        rotate = true;
        StartDoAnima();
    }

    private void OnDisable()
    {
        rotate = false;
    }

    private void StartDoAnima()
    {
        //button.transform.DORotate(new Vector3(0, 0, -byValue / 2), duratation, rotateModel).SetEase(Ease.OutSine).OnComplete(DoGo);
    }
    int count = 0;
    void DoGo()
    {
        count++;
        //button.transform.DORotate(new Vector3(0, 0, byValue), duratation, rotateModel).SetEase(ease).OnComplete(DoBack);
    }

    private void DoBack()
    {
        if (!rotate)
        {
            count = 0;
            button.transform.localRotation = Quaternion.Euler(0, 0, 0);
            return;
        }

        if (count == MaxCount)
        {
            count = 0;
            Wait();
        }
        else
        {
            byValue = -byValue;
            DoGo();
        }
    }
    private void Wait()
    {
        byValue = -byValue;
        //button.transform.DORotate(new Vector3(0, 0, 0), duratation, rotateModel).SetEase(Ease.OutSine).OnComplete(StartDoAnima);
    }
}
