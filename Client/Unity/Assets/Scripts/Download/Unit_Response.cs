using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using STools.Tool_Manager;
public enum DownloadState
{
    None,
    ResetSize,
    Download,
    Md5,
    Complete,
    Error,
}


//断点续传下载文件
//文件下载流程
//网络选型
//断点续传下载逻辑
//异常处理——循环下载逻辑
//MD5校验

//断点续传下载文件
//文件下载流程
//针对文件下载，要保证准确性和高效，设计了如下的流程
//文件下载流程：开始——》获取文件长度——》下载文件——》MD5校验——》下载完成
//                                                               ——》解压文件——》下载完成

/// <summary>
/// //请求的数据结构
/// </summary>
public class Unit_Response
{
    //由于整个下载流程都在线程中，那么直接使用的就是HttpWebRequest的同步接口(阻塞)，等待结果返回。
    //HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
    //HttpWebResponse respone = (HttpWebResponse)request.GetResponse();
    //Stream stream = respone.GetResponseStream();
    //int readSize = stream.Read(bytes, 0, oneReadLen); // 读取第一份数据


    //使用同步接口(阻塞)，一定要设置超时机制，否则默认的超时机制要么超长要么不起效。
    //HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
    //request.Timeout = TimeOutWait;
    //request.ReadWriteTimeout = ReadWriteTimeOut;


    //断点续传下载逻辑
    //断点续传有多种方式实现，比如迅雷的下载，是将大文件切成4MB的块，然后不同线程（p2p）下载对应块，
    //然后所有块下载完成，校验一下，整个大文件就下载完成了。
    //而这里涉及的场景，是对单一路径（CDN）的下载，并不需要那么复杂的逻辑，只需要的是一个可以继续下载的功能。
    //下载逻辑如下图:

    //开始—》临时文件存在—》NO—》创建长度为0的临时文件—》网络下载固定长度—》写入临时文件尾部—》临时文件长度—》MAX—》修改临时文件名为正式文件名—》下载完成
    //                                                                                                           —》No Enough—》网络下载固定长度—》写入临时文件尾部—》临时文件长度—》MAX—》修改临时文件名为正式文件名—》下载完成
    //                        YES—》网络下载固定长度—》写入临时文件尾部—》临时文件长度—》MAX—》修改临时文件名为正式文件名—》下载完成
    //                                                                                                           —》No Enough—》网络下载固定长度—》写入临时文件尾部—》临时文件长度—》MAX—》修改临时文件名为正式文件名—》下载完成


    //TIP：为什么不下载整个文件，然后写整个文件？
    //直接写整个文件，这是很多游戏的暴力做法，但导致的问题会很多
    //文件过大会占用大量内存
    //文件越大，写入失败概率越高
    //进程/线程意外死亡，文件存在但只有一半大小（文件损坏）
    //浪费网络流量，重启需要重新下载

    //TIP：每次下载的固定长度为多少？根据文件系统每个文件块一般大小是4KB（不同文件系统可能不一样），最好是4KB的整数倍，这样能够实现最高效率。这里定义const int oneReadLen = 16 * 1024;，是折中的做法，读者可以自己根据需求定义。
    //TIP：断点续传是怎么实现的？当下载线程重新下载这个文件的时候，可以通过判断对应的临时文件是否存在，并获取临时文件的长度，然后从当前长度开始下载，这样就实现了断点续传。
    const int oneceReadLen = 16 * 1024;       // 一次读取长度 16384 = 16 * 1kb
    const int Md5ReadLen = 16 * 1024;       // 一次读取长度 16384 = 16 * 1kb
    const int TimeOutWait = 5 * 1000;       // 请求超时
    const int ReadWriteTimeOut = 2 * 1000;  // 读写超时

    public int _curSize = 0;
    public int _allSize = 0;
    public int _tryCount = 0;
    public string _error = string.Empty;

    public DownloadState state = DownloadState.None;

    public Unit_Request _unitRequest;

    /// <summary>
    /// 构造方法初始化Unit_Request对象
    /// </summary>
    /// <param name="unit_Request"></param>
    public Unit_Response(Unit_Request unit_Request)
    {
        _unitRequest = unit_Request;
    }

    //防止失败频繁回调，只在特定次数回调
    public bool IsNeedErrorCall()
    {
        if (_tryCount == 3 || _tryCount == 10 || _tryCount == 100)
            return true;
        return false;
    }

    //进行下载
    public void Run()
    {
        _tryCount++;

        state = DownloadState.ResetSize;
        if (!ResetSize())//1、文件大小<=0时，退出
            return;

        state = DownloadState.Download;
        if (!Download())//2、下载文件
            return;

        state = DownloadState.Md5;
        if (!CheckMd5()) //3、校验失败，重下一次
        {
            state = DownloadState.Download;
            if (!Download()) return;

            state = DownloadState.Md5;
            if (!CheckMd5()) return; //4、两次都失败，文件有问题
        }

        state = DownloadState.Complete;//5、下载完成
    }

    private bool ResetSize()
    {
        if (_unitRequest.size <= 0)
        {
            _unitRequest.size = GetWebFileSize(_unitRequest.downloadUrl);

            if (_unitRequest.size == 0)//如果文件大小等于0
                return false;
        }
        _curSize = 0;//设置已下载文件的大小为0
        _allSize = _unitRequest.size;//文件的大小
        return true;
    }


    /// <summary>
    /// 检验MD5
    /// </summary>
    /// <returns></returns>
    private bool CheckMd5()
    {
        if (string.IsNullOrEmpty(_unitRequest.md5))
            return true; //不做校验，默认成功

        string md5 = GetMD5HashFromFile(_unitRequest.localSaveFullPath);

        if (md5 != _unitRequest.md5)
        {
            File.Delete(_unitRequest.localSaveFullPath);
            //ThreadDebugLog.Log("文件MD5校验出错：" + _unitRequest.name);
            state = DownloadState.Error;
            _error = "Check MD5 Error ";
            return false;
        }

        return true;
    }


    /// <summary>
    /// 下载
    /// </summary>
    /// <returns></returns>
    public bool Download()
    {
        if (_unitRequest.streamType == StreamType.General)
        {
            if (_unitRequest.storageType == StorageType.String)
            {
                Debug.Log(_unitRequest.streamType.ToString() + "=>" + _unitRequest.storageType);

                HttpWebRequest rq = null;
                HttpWebResponse rs = null;
                Stream s = null;
                StreamReader sr = null;
                try
                {
                    rq = (HttpWebRequest)WebRequest.Create(_unitRequest.downloadUrl);
                    rs = (HttpWebResponse)rq.GetResponse();
                    s = rs.GetResponseStream();
                    sr = new StreamReader(s, System.Text.Encoding.UTF8);

                    _unitRequest.res_String = sr.ReadToEnd();//直接转为字符串
                }
                catch (Exception ex)
                {
                    state = DownloadState.Error;
                    _error = "Download Error " + ex.Message;
                }
                finally
                {
                    if (sr != null)
                    {
                        sr.Dispose();
                        sr.Close();
                    }
                    if (s != null)
                    {
                        s.Dispose();
                        s.Close();
                    }
                    if (rs != null)
                    {
                        rs.Close();
                    }
                    if (rq != null)
                    {
                        rq.Abort();
                    }
                }
                if (state == DownloadState.Error)
                {
                    return false;
                }
                return true;
            }
            else
            {
                return true;
            }
        }
        else if (_unitRequest.streamType == StreamType.Memory)
        {
            if (_unitRequest.storageType == StorageType.Texture)
            {
                Debug.Log(_unitRequest.streamType.ToString() + "=>" + _unitRequest.storageType.ToString());

                HttpWebRequest rq = null;
                HttpWebResponse rs = null;
                Stream s = null;
                MemoryStream ms = null;

                try
                {
                    rq = (HttpWebRequest)WebRequest.Create(_unitRequest.downloadUrl);
                    rs = (HttpWebResponse)rq.GetResponse();

                    s = rs.GetResponseStream();

                    ms = Utility_Download.StreamToMemoryStream(s);

                    _unitRequest.res_Bytes = ms.ToArray();

                }
                catch (Exception ex)
                {
                    state = DownloadState.Error;
                    _error = "Download Error " + ex.Message;
                }
                finally
                {
                    if (ms != null)
                    {
                        ms.Dispose();
                        ms.Close();
                    }

                    if (s != null)
                    {
                        s.Dispose();
                        s.Close();
                        s = null;
                    }

                    if (rs != null)
                    {
                        rs.Close();
                        rs = null;
                    }

                    if (rq != null)
                    {
                        rq.Abort();
                        rq = null;
                    }
                }
                if (state == DownloadState.Error)
                    return false;
                return true;
            }
            else if (_unitRequest.storageType == StorageType.AssetBundle)
            {
                Debug.Log(_unitRequest.streamType.ToString() + "=>" + _unitRequest.storageType.ToString());

                HttpWebRequest rq = null;
                HttpWebResponse rs = null;
                Stream s = null;
                MemoryStream ms = null;
                
                try
                {
                    rq = (HttpWebRequest)WebRequest.Create(_unitRequest.downloadUrl);
                    rs = (HttpWebResponse)rq.GetResponse();
                    s = rs.GetResponseStream();
                    ms = Utility_Download.StreamToMemoryStream(s);
                    _unitRequest.res_Stream = ms;
                }
                catch (Exception ex)
                {
                    state = DownloadState.Error;
                    _error = "Download Error " + ex.Message;
                }
                finally
                {
                    if (s != null)
                    {
                        s.Dispose();
                        s.Close();
                        s = null;
                    }

                    if (rs != null)
                    {
                        rs.Close();
                        rs = null;
                    }

                    if (rq != null)
                    {
                        rq.Abort();
                        rq = null;
                    }
                }
                if (state == DownloadState.Error)
                {
                    return false;
                }
                return true;

            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }


    /// <summary>
    /// 从文件中获取MD5哈希
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private string GetMD5HashFromFile(string fileName)
    {
        byte[] buffer = new byte[Md5ReadLen];

        int readLength = 0;//每次读取长度

        var output = new byte[Md5ReadLen];

        using (Stream inputStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using (System.Security.Cryptography.HashAlgorithm hashAlgorithm = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                while ((readLength = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    //计算MD5  块转换
                    hashAlgorithm.TransformBlock(buffer, 0, readLength, output, 0);//文件分段读取，节省内存。
                }

                //完成最后计算，必须调用(由于上一部循环已经完成所有运算，所以调用此方法时后面的两个参数都为0)  
                hashAlgorithm.TransformFinalBlock(buffer, 0, 0);

                byte[] retVal = hashAlgorithm.Hash;

                System.Text.StringBuilder sb = new System.Text.StringBuilder(32);

                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));//即转化为小写的16进制。
                }

                hashAlgorithm.Clear();//清除
                inputStream.Close();//关闭操作流
                return sb.ToString();
            }
        }
    }

    /// <summary>
    /// 获得要下载的文件大小
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private int GetWebFileSize(string url)
    {
        HttpWebRequest request = null;
        WebResponse respone = null;
        int length = 0;

        try
        {
            request = WebRequest.Create(url) as HttpWebRequest;
            request.Timeout = TimeOutWait;//设置超时等待时间
            request.ReadWriteTimeout = ReadWriteTimeOut;//设置读写超时等待时间

            //向服务器发送请求，获得服务器回应数据流
            respone = request.GetResponse();
            length = (int)respone.ContentLength;//获得文件长度
        }
        catch (WebException e)
        {
            //ThreadDebugLog.Log("获取文件长度出错：" + e.Message);
            state = DownloadState.Error;
            _error = "Request File Length Error " + e.Message;
        }
        finally
        {
            if (respone != null)//释放
            {
                respone.Close();
                respone = null;
            }
            if (request != null)//释放
            {
                request.Abort();
                request = null;
            }
        }
        return length;
    }

}


