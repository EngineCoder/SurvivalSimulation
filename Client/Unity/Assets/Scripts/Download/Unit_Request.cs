using System.IO;
using UnityEngine;
using STools.Tool_Manager;
/// <summary>
/// 上传的数据结构(对象)
/// </summary>
public class Unit_Request
{
    public StreamType streamType = StreamType.General;
    public StorageType storageType = StorageType.String;//(必须)
    public string downloadUrl;//下载地址。（必须）

    public string localSaveFullPath;//本地完整路径。（必须）
    public string fileName;//文件名。（必须）
    public bool isDelete;//用于清理正在下载的文件

    public int size;//文件长度，非必须
    public string md5;//需要校验的md5,非必须


    #region Result
    public string res_String;
    public byte[] res_Bytes;
    public Stream res_Stream;
    #endregion


    public delegate void DownLoadCallBack_Progress(Unit_Request unit_Request, int curSize, int allSize);//下载进度回调
    public delegate void DownLoadCallBack_Complete(Unit_Request unit_Request);//下载完成回调
    public delegate void DownLoadCallBack_Error(Unit_Request unit_Request, string msg);//下载错误回调

    public DownLoadCallBack_Progress progressFunction = null;//进度回调
    public DownLoadCallBack_Complete completeFunction = null;//完成回调
    public DownLoadCallBack_Error errorFunction = null;//错误回调
}