using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Tools : MonoBehaviour
{

    /// <summary>
    /// 判断输入的字符串是否是一个合法的手机号
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsMobilePhone(string input)
    {
        Regex regex = new System.Text.RegularExpressions.Regex("^1[34578]\\d{9}$");
        return regex.IsMatch(input);
    }
}
