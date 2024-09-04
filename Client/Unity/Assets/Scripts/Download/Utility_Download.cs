using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;

public class Utility_Download
{
    #region

    /// <summary>
    /// 返回字符串
    /// </summary>
    /// <param name="url"></param>
    /// <param name="timeout"></param>
    /// <returns>string</returns>
    public static string DownloadByHWR_String(string url, int? timeout = 5000)
    {
        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
        request.Method = "GET";

        if (timeout.HasValue)
        {
            request.Timeout = timeout.Value;
        }

        using (HttpWebResponse resResponse = (HttpWebResponse)request.GetResponse())
        {
            using (Stream resStream = resResponse.GetResponseStream())
            {
                using (StreamReader resStreamReader = new StreamReader(resStream, System.Text.Encoding.UTF8))
                {
                    return resStreamReader.ReadToEnd();
                }
            }
        }
    }

    /// <summary>
    /// 下载文件到本地
    /// </summary>
    /// <param name="url"></param>
    /// <param name="filename"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static bool DownloadByHWR_File(string url, string filename, string dir, int? timeout = 5000)
    {
        try
        {
            string fullPath = LocalSaveFullPath(filename, dir);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            FileStream fs = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

            //设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }

            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            HttpWebResponse webResponse = request.GetResponse() as HttpWebResponse;

            Debug.Log(webResponse.ContentLength);

            Stream resSponseStream = webResponse.GetResponseStream();

            //创建本地文件写入流
            byte[] bArr = new byte[1024];

            int totalSize = 0;

            int size = resSponseStream.Read(bArr, 0, (int)bArr.Length);

            while (size > 0)
            {
                totalSize += size;

                fs.Write(bArr, 0, size);

                size = resSponseStream.Read(bArr, 0, (int)bArr.Length);
            }
            fs.Close();
            resSponseStream.Close();
            return true;
        }
        catch (Exception)
        {
            return false;
        }

    }

    /// <summary> 
    /// 创建POST方式的HTTP请求 
    /// </summary> 
    /// <param name="url">请求的URL</param> 
    /// <param name="parameters">随同请求POST的参数名称及参数值字典</param> 
    /// <param name="timeout">请求的超时时间</param> 
    /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param> 
    /// <param name="requestEncoding">发送HTTP请求时所用的编码</param> 
    /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param> 
    /// <returns></returns> 
    public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentNullException("url");
        }
        if (requestEncoding == null)
        {
            throw new ArgumentNullException("requestEncoding");
        }

        HttpWebRequest request = null;

        //如果是发送HTTPS请求 
        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            request = WebRequest.Create(url) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;
        }
        else
        {
            request = WebRequest.Create(url) as HttpWebRequest;
        }
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        if (timeout.HasValue)
        {
            request.Timeout = timeout.Value;
        }

        //如果需要POST数据 
        if (!(parameters == null || parameters.Count == 0))
        {
            StringBuilder buffer = new StringBuilder();
            int i = 0;
            foreach (string key in parameters.Keys)
            {
                if (i > 0)
                {
                    buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                }
                else
                {
                    buffer.AppendFormat("{0}={1}", key, parameters[key]);
                }
                i++;
            }
            byte[] data = requestEncoding.GetBytes(buffer.ToString());
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
        }
        return request.GetResponse() as HttpWebResponse;
    }
    #endregion

    /// <summary>
    /// 给定文件名和文件夹名字,创建本地保存路径。
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="dic"></param>
    /// <returns></returns>
    public static string LocalSaveFullPath(string filename, string directory)
    {
        if (Application.platform == RuntimePlatform.Android)//如果是安卓
        {
            //存数据
            //Application.persistentDataPath+""
            //取数据
            //"file://"+Application.persistentDataPath+"/dic/+filename;

            string path = Application.persistentDataPath;//  /data/data/xxx.xxx.xxx/files
            path = path.Substring(0, path.LastIndexOf('/'));//  /data/data/xxx.xxx.xxx/
            path = Path.Combine(path, directory);//  /data/data/xxx.xxx.xxx/sufo

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return Path.Combine(path, filename);//  /data/data/xxx.xxx.xxx/sufo/520.png
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)//如果是苹果
        {
            //Application.dataPath : Application / xxxxxxxx - xxxx - xxxx - xxxx - xxxxxxxxxxxx / xxx.app / Data
            //Application.streamingAssetsPath : Application / xxxxxxxx - xxxx - xxxx - xxxx - xxxxxxxxxxxx / xxx.app / Data / Raw
            //Application.persistentDataPath :    Application / xxxxxxxx - xxxx - xxxx - xxxx - xxxxxxxxxxxx / Documents
            //Application.temporaryCachePath : Application / xxxxxxxx - xxxx - xxxx - xxxx - xxxxxxxxxxxx / Library / Caches

            string path = Application.persistentDataPath; //Application/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/Docu
            path = path.Substring(0, path.LastIndexOf('/'));
            path = Path.Combine(path, directory);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return Path.Combine(path, filename);
        }
        else
        {
            string path = Application.dataPath;
            //path = path.Substring(0, path.LastIndexOf('/'));
            path = Path.Combine(path, directory);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return Path.Combine(path, filename);
        }
    }

    /// <summary>
    /// Stream To MemoryStream  网络流转化为内存流
    /// </summary>
    public static MemoryStream StreamToMemoryStream(Stream inStream)
    {
        MemoryStream outStream = new MemoryStream();

        const int bufferLen = 4 * 1024;
        byte[] buffer = new byte[bufferLen];
        int count = 0;
        while ((count = inStream.Read(buffer, 0, bufferLen)) > 0)
        {
            outStream.Write(buffer, 0, count);
        }
        return outStream;
    }


    private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
    {
        return true; //总是接受 
    }

//            else if (_unitRequest.streamType == StreamType.FileStream)
//        {
//            if (_unitRequest.storageType == StorageType.Texture)
//            {
//                long startPos = 0;
//    string tempFile = _unitRequest.localSaveFullPath + ".ysdownload";//0、创建临时文件路径

//    FileStream fs = null;

//                if (File.Exists(_unitRequest.localSaveFullPath))//1、文件已下载则退出
//                {
//                    _curSize = _unitRequest.size;
//                    return true;
//                }
//                else if (File.Exists(tempFile))//2、文件没有下载，但存在临时文件
//                {
//                    fs = File.OpenWrite(tempFile);//3、打开临时文件，进行写操作

//                    startPos = fs.Length;//4、设置写入的初始位置（当前临时文件的大小）

//                    fs.Seek(startPos, SeekOrigin.Current); //5、移动当前指针到startPos位置。

//                    if (startPos == _unitRequest.size)//6、文件下载完毕
//                    {
//                        fs.Flush();//7、释放

//                        fs.Close();//8、关闭

//                        fs = null;//9、置空

//                        if (File.Exists(_unitRequest.localSaveFullPath))//10、如果文件已存在，删除该文件
//                            File.Delete(_unitRequest.localSaveFullPath);

//                        File.Move(tempFile, _unitRequest.localSaveFullPath);//11、将临时文件移动到localSaveFullPath 文件名已完成更改

//                        _curSize = (int) startPos;
//                        return true;
//                    }
//                }
//                else
//                {
//                    string direName = Path.GetDirectoryName(tempFile);//12、获得文件存放的路径名

//                    if (!Directory.Exists(direName))//13、路径不存在则创建路径
//                        Directory.CreateDirectory(direName);

//                    fs = new FileStream(tempFile, FileMode.Create);//14、创建文件
//                }

//                // 下载逻辑，如果当前是临时文件，则当前指针已经移动到startPos位置，临时文件的末尾位置
//                HttpWebRequest request = null;
//HttpWebResponse respone = null;

//Stream stream = null;
//                try
//                {
//                    request = WebRequest.Create(_unitRequest.downloadUrl) as HttpWebRequest;
//                    request.Timeout = TimeOutWait;
//                    request.ReadWriteTimeout = ReadWriteTimeOut;

//                    if (startPos > 0)//15、如果startPos位置大于0则表明存在临时文件
//                        request.AddRange((int) startPos);  //16、设置Range值，断点续传

//                    respone = (HttpWebResponse) request.GetResponse();//17、向服务器请求，获得服务器回应数据流
//stream = respone.GetResponseStream();
//                    stream.ReadTimeout = TimeOutWait;

//                    long totalSize = respone.ContentLength;//18、获得下载的内容大小
//long curSize = startPos;

//                    //Debug.Log("=====curSize======" + curSize);

//                    if (curSize == totalSize)
//                    {
//                        fs.Flush();
//                        fs.Close();
//                        fs = null;

//                        if (File.Exists(_unitRequest.localSaveFullPath))
//                            File.Delete(_unitRequest.localSaveFullPath);

//                        File.Move(tempFile, _unitRequest.localSaveFullPath);
//                        _curSize = (int) curSize;
//                    }
//                    else
//                    {
//                        byte[] bytes = new byte[oneceReadLen];//设置每次读取的长度

//int readSize = stream.Read(bytes, 0, oneceReadLen); // 读取第一份数据

//                        while (readSize > 0)
//                        {
//                            fs.Write(bytes, 0, readSize);       // 将下载到的数据写入临时文件

//                            curSize += readSize;

//                            //Debug.Log("=====curSize======" + curSize);
//                            //Debug.Log("======totalSize=====" + totalSize);

//                            // 判断是否下载完成
//                            if (curSize == totalSize)
//                            {
//                                fs.Flush();
//                                fs.Close();
//                                fs = null;

//                                if (File.Exists(_unitRequest.localSaveFullPath))
//                                    File.Delete(_unitRequest.localSaveFullPath);

//                                File.Move(tempFile, _unitRequest.localSaveFullPath); //下载完成,将temp文件改成正式文件
//                            }

//                            // 回调一下
//                            _curSize = (int) curSize;

//// 往下继续读取
//readSize = stream.Read(bytes, 0, oneceReadLen);

//                            //Debug.Log("======readSize=====" + readSize);
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Debug.Log(ex.Message);

//                    //下载失败，删除临时文件
//                    if (fs != null)
//                    {
//                        fs.Flush();
//                        fs.Close();
//                        fs = null;
//                    }

//                    if (File.Exists(tempFile))
//                        File.Delete(tempFile);

//                    if (File.Exists(_unitRequest.localSaveFullPath))
//                        File.Delete(_unitRequest.localSaveFullPath);

//                    //ThreadDebugLog.Log("下载出错：" + ex.Message);

//                    state = DownloadState.Error;
//                    _error = "Download Error " + ex.Message;
//                }
//                finally
//                {
//                    if (fs != null)
//                    {
//                        fs.Flush();
//                        fs.Close();
//                        fs = null;
//                    }

//                    if (stream != null)
//                    {
//                        stream.Close();
//                        stream = null;
//                    }

//                    if (respone != null)
//                    {
//                        respone.Close();
//                        respone = null;
//                    }

//                    if (request != null)
//                    {
//                        request.Abort();
//                        request = null;
//                    }
//                }
//                if (state == DownloadState.Error)
//                    return false;
//                return true;
//            }
//            else if (_unitRequest.storageType == StorageType.AssetBundle)
//            {
//                Debug.Log(StorageType.String.ToString());

//                HttpWebRequest rq = null;
//HttpWebResponse rs = null;
//Stream s = null;
//StreamReader sr = null;

//                try
//                {
//                    HttpWebRequest hrq = (HttpWebRequest)WebRequest.Create(_unitRequest.downloadUrl);
//HttpWebResponse hrs = (HttpWebResponse)hrq.GetResponse();

//s = hrs.GetResponseStream();
//                    sr = new StreamReader(s, System.Text.Encoding.UTF8);
//_unitRequest.res_String = sr.ReadToEnd();
//                }
//                catch (Exception ex)
//                {
//                    state = DownloadState.Error;
//                    _error = "Download Error " + ex.Message;
//                }
//                finally
//                {
//                    if (sr != null)
//                    {
//                        sr.Dispose();
//                        sr.Close();
//                    }
//                    if (s != null)
//                    {
//                        s.Dispose();
//                        s.Close();
//                    }
//                    if (rs != null)
//                    {
//                        rs.Close();
//                    }
//                    if (rq != null)
//                    {
//                        rq.Abort();
//                    }
//                }

//                if (state == DownloadState.Error)
//                {
//                    return false;
//                }
//                return true;

//            }
//            else
//            {
//                return true;
//            }
//        }









}