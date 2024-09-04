using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Sort : MonoBehaviour
{
    public string name;
    public string intro;
    // Start is called before the first frame update
    void Start()
    {
        //Dictionary<int, Transform> keyValuePairs = new Dictionary<int, Transform>();
        //foreach (Transform item in transform)
        //{
        //    keyValuePairs.Add(int.Parse(item.name.Substring(item.name.LastIndexOf('_') + 1)), item);
        //}

        //int[] keys = keyValuePairs.Keys.ToArray();

        //Array.Sort(keys);

        //for (int i = 0; i < keys.Length; i++)
        //{
        //    keyValuePairs[keys[i]].SetAsLastSibling();
        //}

        TextAsset textAsset = Resources.Load<TextAsset>("TextAsset/image_Intro");

        string[] str = null;

        if (textAsset.text.Contains('\n'))
        {
            str = textAsset.text.Split('\n');
        }
        else if (textAsset.text.Contains('\r'))
        {
            str = textAsset.text.Split('\r');
        }
        else
        {
            Debug.Log("不含有换行符");
        }

        Debug.Log(str[0]);
        Debug.Log(str[1]);

        //Debug.Log(LoadTextReader(Application.dataPath + @"\image_Intro.txt", 1));
    }
    
    public string LoadTextReader(string fullPath, int row)
    {
        string str = string.Empty;
        using (TextReader tr = new StreamReader(fullPath)) //Application.dataPath + @"\image_Intro.txt")
        {
            int i = 1;
            while (true)
            {
                str = tr.ReadLine();
                if (string.IsNullOrEmpty(str))
                {
                    break;
                }
                else
                {
                    if (i == row)
                    {
                        break;
                    }
                    i++;
                }
            }
            tr.Close();
        }
        return str;
    }
}
