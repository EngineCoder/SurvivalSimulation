using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;

namespace STools.Tool_Manager
{
    // 在游戏开发中，热更并下载资源，对商业化游戏来说是一个必须的需求。
    // 而想要高效并稳定地下载文件一直是开发中的一个痛点。
    // 我们能看到市面上，大量游戏都在下载上卡死，断网，重启无效等等问题。
    // 我们来总结一下要解决的问题：对于版本管理导致的问题，下一篇文章再讨论。
    //1、网络请求异常处理——断网、请求失败、请求超时、网络波动、下载到一半等问题
    //2、文件读写异常处理——文件读失败、文件写失败、文件写一半等问题
    //3、游戏进程异常处理——下载到一半、文件写一半、玩家退出等问题
    //4、重启现场恢复处理——可恢复性，继续下载
    //5、文件的下载正确性——文件长度、文件校验，文件可读性
    //6、文件线程高效下载——多线程，异步回调文件

    public enum StreamType
    {
        General,
        Memory,
        File
    }
    public enum StorageType
    {
        String,
        Texture,
        AssetBundle
    }

    /// <summary>
    /// 下载管理
    /// <para>1、下载时请求的数据结构</para>
    /// <para>2、下载管理框架代码</para>
    /// <para>3、多线程运行逻辑</para>
    /// <para>4、下载回调和线程遍历</para>
    /// <para>5、网络相关</para>
    /// </summary>
    public class Manager_Download
    {
        private static Manager_Download _intance = null;
        public static Manager_Download Instance
        {
            get
            {
                if (_intance == null)
                    _intance = new Manager_Download();
                return _intance;
            }
        }



        #region 多线程运行逻辑
        //1、线程的数量控制——内存占用上限，同时运行数量
        //2、线程的重复利用——降低线程创建销毁的开销
        //3、线程内存的释放——下载结束，释放线程，不占用内存
        #endregion

        //1、既然是多线程，那么上面的锁_lock就是必须的，解决竞争问题。这边_lock是唯一的，必然不会有死锁问题。
        private static object _lock = new object();

        //2、线程的数量控制，内存占用上限，同时运行数量
        private const int MAX_THREAD_COUNT = 20;

        private Queue<Unit_Response> queue_Read;//请求队列
        private Dictionary<Thread, Unit_Response> dict_Running;//下载运行队列
        private List<Unit_Request> list_Complete;//下载完成队列
        private List<Unit_Response> list_Error;//下载异常队列


        /// <summary>
        /// 构造方法，初始化
        /// </summary>
        private Manager_Download()
        {
            queue_Read = new Queue<Unit_Response>();//要下载的队列
            dict_Running = new Dictionary<Thread, Unit_Response>();//下载中的队列
            list_Complete = new List<Unit_Request>();//下载完成的队列
            list_Error = new List<Unit_Response>();//下载错误的队列

            //https解析的设置
            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
        }



        #region 下载文件
        //1、异步下载，创建线程
        public void Download_Async(Unit_Request unit_Request)
        {
            if (unit_Request == null)//1、请求单位为空，则直接返回
                return;

            var unit_Response = new Unit_Response(unit_Request);//2、通过请求单位构造响应单位

            lock (_lock)//加锁
            {
                queue_Read.Enqueue(unit_Response);//3、将响应的单位，加入到读队列
            }

            if (dict_Running.Count < MAX_THREAD_COUNT)//4、运行队列中运行的个数小于最大线程数时
            {
                var thread = new Thread(ThreadLoop);//5、通过线程遍历,创建线程

                lock (_lock)
                {
                    dict_Running.Add(thread, null);//6、从读队列中读取要下载的对象，加入到运行队列中，进行下载
                }
                thread.Start();
            }
        }

        //2、线程遍历，三个逻辑，关闭卡死线程、网络断开处理和开启新线程。
        private void ThreadLoop()
        {
            while (true)
            {
                Unit_Response unit = null;//1、声明一个临时响应单位

                lock (_lock)//加锁
                {
                    if (queue_Read.Count > 0)//2、读队列中有响应单位时
                    {
                        unit = queue_Read.Dequeue();//3、取出响应单位

                        dict_Running[Thread.CurrentThread] = unit;//4、将该任务添加到运行队列

                        if (unit != null && unit._unitRequest.isDelete)//5、已经销毁，不提取运行，直接删除
                        {
                            dict_Running[Thread.CurrentThread] = null;
                            continue;
                        }
                    }
                }

                if (unit == null) //6、读队列中已经没有需要下载的了
                    break;

                unit.Run();//7、运行进行下载

                if (unit.state == DownloadState.Complete)//8、下载完成
                {
                    lock (_lock)//加锁
                    {
                        list_Complete.Add(unit._unitRequest);//9、则添加该任务到完成队列中

                        dict_Running[Thread.CurrentThread] = null;//10、将运行队列中当前运行线程对应的下载任务置空
                    }
                }
                else if (unit.state == DownloadState.Error)//11、下载失败
                {
                    lock (_lock)
                    {
                        queue_Read.Enqueue(unit);//12、下载失败，重新放入下载队列，下次继续下载

                        if (unit.IsNeedErrorCall())//13、防止失败频繁回调，只在特定次数回调
                        {
                            list_Error.Add(unit);
                        }
                    }
                    break;
                }
                else
                {
                    Debug.Log("该任务下载失败" + "Error DownloadState " + unit.state + " " + unit._unitRequest.fileName);
                    //ThreadDebugLog.Log("Error DownloadState " + mac.state + " " + mac.downloadUnit.name);
                    break;
                }
            }
        }

        //3、同步下载，不会调用回调函数，返回是否成功
        public bool Dowload_Sync(Unit_Request unitRequest)
        {
            if (unitRequest == null)
            {
                return false;
            }

            var unit = new Unit_Response(unitRequest);

            try
            {
                //同步下载尝试三次
                unit.Run();
                if (unit.state == DownloadState.Complete)
                    return true;

                unit.Run();
                if (unit.state == DownloadState.Complete)
                    return true;

                unit.Run();
                if (unit.state == DownloadState.Complete)
                    return true;
            }
            catch (Exception ex)
            {
                Debug.Log("Error DownloadSync " + unit.state + " " + unit._unitRequest.fileName + " " + ex.Message + " " + ex.StackTrace);
            }
            return false;
        }
        #endregion



        #region 更新回调
        //下载回调刷新
        public void Update_Download()
        {
            UpdateComplete();
            UpdateProgress();
            UpdateError();
            UpdateThread();
        }

        //更新完成队列的回调
        private void UpdateComplete()
        {
            if (list_Complete.Count == 0)
            {
                return;
            }

            Unit_Request[] completeArr = null;

            lock (_lock)
            {
                completeArr = list_Complete.ToArray();
                list_Complete.Clear();
            }

            foreach (var item in completeArr)
            {
                if (item.isDelete)
                {
                    continue;//已经销毁，不进行回调
                }

                item.isDelete = true;

                if (item.progressFunction != null)//下载进度回调
                {
                    item.progressFunction(item, item.size, item.size);
                }

                if (item.completeFunction != null)//下载完成回调
                {
                    item.completeFunction(item);
                }
            }
        }

        //更新错误队列的回调
        private void UpdateError()
        {
            if (list_Error.Count == 0) return;

            Unit_Response[] errorArr = null;

            lock (_lock)
            {
                errorArr = list_Error.ToArray();
                list_Error.Clear();
            }
            foreach (var unit in errorArr)
            {
                var info = unit._unitRequest;
                if (info.isDelete)
                    continue; //已经销毁，不进行回调

                if (info.errorFunction != null)
                {
                    info.errorFunction(info, unit._error);
                    unit._error = "";
                }
            }
        }

        //更新进度队列的回调
        private void UpdateProgress()
        {
            //回调进度
            if (dict_Running.Count == 0)
                return;

            List<Unit_Response> runArr = new List<Unit_Response>();
            lock (_lock)
            {
                foreach (var mac in dict_Running.Values)
                {
                    if (mac != null) runArr.Add(mac);
                }
            }

            foreach (var mac in runArr)
            {
                var info = mac._unitRequest;
                if (info.isDelete) continue; //已经销毁，不进行回调

                if (info.progressFunction != null)
                {
                    info.progressFunction(info, mac._curSize, mac._allSize);
                }
            }
        }

        //线程更新
        private void UpdateThread()
        {
            //1、没有下载任务了，则退出
            if (queue_Read.Count == 0 && dict_Running.Count == 0)
            {
                return;
            }

            lock (_lock)
            {
                //暂存卡死的线程,从而进行关闭
                List<Thread> list_BrokenThread = new List<Thread>();

                foreach (var item_Running in dict_Running)
                {
                    if (item_Running.Key.IsAlive)//线程活着
                    {
                        continue;
                    }

                    //线程嗝屁了。但是还没下载完，重新入请求队列
                    if (item_Running.Value != null)
                    {
                        queue_Read.Enqueue(item_Running.Value);
                    }

                    //将嗝屁的线程添加到list_BrokenThread
                    list_BrokenThread.Add(item_Running.Key);
                }

                foreach (var thread in list_BrokenThread)
                {
                    dict_Running.Remove(thread);
                    thread.Abort();
                }
            }

            //如果没有网络，不开启新线程，旧线程会逐个关闭
            if (!CheckNetworkActive())
            {
                return;
            }

            //线程数量不足，创建
            if (dict_Running.Count >= MAX_THREAD_COUNT)
            {
                return;
            }

            if (queue_Read.Count > 0)
            {
                var thread = new Thread(ThreadLoop);
                lock (_lock)
                {
                    dict_Running.Add(thread, null);
                }
                thread.Start();
            }
        }
        #endregion



        #region 标记为删除
        //将某个文件标记为要删除的文件
        public void DeleteDownload(Unit_Request unitRequest)
        {
            lock (_lock)
            {
                unitRequest.isDelete = true;
            }
        }

        //将所有的文件标记为要删除的文件
        public void ClearAllDownloads()
        {
            lock (_lock)
            {
                foreach (var unit in queue_Read)
                {
                    if (unit != null) unit._unitRequest.isDelete = true;
                }

                foreach (var unit in dict_Running)
                {
                    if (unit.Value != null) unit.Value._unitRequest.isDelete = true;
                }

                foreach (var unit in list_Complete)
                {
                    if (unit != null) unit.isDelete = true;
                }
            }
        }
        #endregion


        //MemoryStream ms = StreamToMemoryStream(stream);
        //ms.Seek(0, SeekOrigin.Begin);
        //        int buffsize = (int)ms.Length; //rs.Length 此流不支持查找,先转为MemoryStream
        //byte[] bytes = new byte[buffsize];
        //ms.Read(bytes, 0, buffsize);
        //        ms.Flush();
        //        ms.Close();
        //        stream.Flush();
        //        stream.Close();


        /// <summary>
        /// 这里是Unity提供的网络状态接口，针对不同平台，读者可以通过原生平台实现。
        /// </summary>
        /// <returns></returns>
        public bool CheckNetworkActive()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {//没有网络
                return false;
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {//2345G网络
                return true;
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {//wifi网络
                return true;
            }
            return false;
        }


        /// <summary>
        /// 服务器证书验证回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private bool RemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }
    }

}