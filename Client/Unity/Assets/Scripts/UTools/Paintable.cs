using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class Paintable : MonoBehaviour {

    public GameObject brush;
    public float brushSize = 0.1f;
    public RenderTexture renderTexture;


	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButton(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray,out hit))
            {
                Debug.Log(hit.transform.name);
                var go = (GameObject)Instantiate(brush, hit.point + Vector3.up * 0.1f, Quaternion.identity);
                go.transform.SetParent(this.transform);
                go.transform.localScale = Vector3.one * brushSize;
            }
        }
	}

    public void Save()
    {
        StartCoroutine(CoSave());
    }

    IEnumerator CoSave()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log(Application.dataPath+"/SaveImage.png");

        RenderTexture.active = renderTexture;

        var texture2D = new Texture2D(renderTexture.width,renderTexture.height);
        texture2D.ReadPixels(new Rect(0,0,renderTexture.width,renderTexture.height),0,0);
        texture2D.Apply();

        var data = texture2D.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/SaveImage.png", data);
    }
}
