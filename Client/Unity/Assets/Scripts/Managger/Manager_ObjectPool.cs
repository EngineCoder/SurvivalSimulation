using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

namespace STools.Tool_Manager
{
    public class Manager_ObjectPool : SingleMode<Manager_ObjectPool>
    {
        /// <summary>
        /// 所有的对象池
        /// </summary>
        private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();
        public Dictionary<string, ObjectPool> Pools { get { return pools; } set { pools = value; } }


        /// <summary>
        /// 暂存已使用的池子里的对象
        /// </summary>
        private Queue<PoolObject> poolObjects = new Queue<PoolObject>();


        [HideInInspector]
        public List<PoolInfo> poolInfos = new List<PoolInfo>();

        public void Initial()
        {
            LoadAndInitPoolInfoByXmlFile();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PoolObject poolObject = GetObjectFromPool("1", Vector3.one, Quaternion.identity);
                if (poolObject != null)
                {
                    poolObjects.Enqueue(poolObject);
                }
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (poolObjects.Count > 0)
                {
                    ReturnObjectToPool(poolObjects.Dequeue());
                }
            }
        }

        /// <summary>
        /// 初始化，检测对象池数据信息是否重名，创建对象池
        /// </summary>
        public void Init()
        {
            //检测对象池是否重名 | check for duplicate names
            CheckForDuplicatePoolNames();

            //创建对象池 | create pools
            CreatePools();
        }

        /// <summary>
        /// 检测对象池是否重名
        /// </summary>
        private void CheckForDuplicatePoolNames()
        {
            for (int index = 0; index < poolInfos.Count; index++)
            {
                string poolName = poolInfos[index].poolName;
                if (poolName.Length == 0)
                {
                    //Debug.LogError(string.Format("Pool {0} does not have a name!", index));
                }
                for (int internalIndex = index + 1; internalIndex < poolInfos.Count; internalIndex++)
                {
                    if (poolName.Equals(poolInfos[internalIndex].poolName))
                    {
                        //Debug.LogError(string.Format("Pool {0} & {1} have the same name. Assign different names.", index, internalIndex));
                    }
                }
            }
        }

        /// <summary>
        /// 创建对象池
        /// </summary>
        private void CreatePools()
        {
            foreach (PoolInfo currentPoolInfo in poolInfos)
            {
                ObjectPool pool = new ObjectPool(currentPoolInfo.poolName, currentPoolInfo.prefab, currentPoolInfo.poolSize, currentPoolInfo.fixedSize);
                //Debug.Log("Creating Pool: " + currentPoolInfo.poolName + "    PoolSize: " + currentPoolInfo.poolSize + "    FixedSize: " + currentPoolInfo.fixedSize);
                Pools.Add(currentPoolInfo.poolName, pool);
            }
        }


        /// <summary>
        /// 从池中获取一个对象并返回
        ///  null in case the pool does not have any object available & can grow size is false.
        /// </summary>
        /// <param name="poolName">名称</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">rotation</param>
        /// <returns></returns>
        public PoolObject GetObjectFromPool(string poolName, Vector3 position, Quaternion rotation)
        {
            PoolObject result = null;
            if (Pools.ContainsKey(poolName))
            {
                ObjectPool pool = Pools[poolName];
                result = pool.NextAvailableObject(position, rotation);
                //scenario when no available object is found in pool
                if (result == null)
                {
                    //Debug.LogWarning("No object available in pool. Consider setting fixedSize to false.: " + poolName);
                }
            }
            else
            {
                //Debug.LogError("Invalid pool name specified: " + poolName);
            }
            return result;
        }

        /// <summary>
        /// 将不用的对象，放到池子里
        /// </summary>
        /// <param name="po"></param>
        public void ReturnObjectToPool(PoolObject po)
        {
            //PoolObject po = go.GetComponent<PoolObject>();
            if (po == null)
            {
                //Debug.LogWarning("Specified object is not a pooled instance: " + po.name);
            }
            else
            {
                if (Pools.ContainsKey(po.poolName))
                {
                    ObjectPool pool = Pools[po.poolName];
                    pool.ReturnObjectToPool(po);
                }
                else
                {
                    //Debug.LogWarning("No pool available with name: " + po.poolName);
                }
            }
        }


        /// <summary>
        /// 通过加载对象池信息配置文件，初始化对象池的信息
        /// </summary>
        private void LoadAndInitPoolInfoByXmlFile()
        {
            TextAsset xmlTextAsset = Manager_Resources.Instance.Load<TextAsset>(ResourcesType.TextAsset, ResType_TextAsset.Xml.PoolInfo);

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlTextAsset.text);

            XmlNode rootNode = xmlDocument.SelectSingleNode("PoolInfo");//<PoolInfo></PoolInfo>

            string innerXmlInfo = rootNode.InnerXml.ToString();
            string outerXmlInfo = rootNode.OuterXml.ToString();

            //获得该节点的子节点（即：该节点的第一层子节点）
            XmlNodeList xmlNodeList = rootNode.ChildNodes;//<PoolList></PoolList>

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                //获得该节点的属性集合
                XmlAttributeCollection xmlAttributeCollectionRoot = xmlNode.Attributes;

                foreach (XmlAttribute xmlAttribute in xmlAttributeCollectionRoot)
                {
                    string name = xmlAttribute.Name;
                    string value = xmlAttribute.Value;
                    //Debug.LogFormat("{0} = {1}", name, value);
                }

                //判断此节点是否还有子节点
                if (xmlNode.HasChildNodes)
                {

                    XmlNodeList xmlNodeList1 = xmlNode.ChildNodes;
                    foreach (XmlNode xmlNode1 in xmlNodeList1)
                    {
                        PoolInfo poolInfo = new PoolInfo();
                        //获得该节点的属性集合
                        XmlAttributeCollection xmlAttributeCollection1 = xmlNode1.Attributes;
                        foreach (XmlAttribute xmlAttribute1 in xmlAttributeCollection1)
                        {
                            switch (xmlAttribute1.Name)
                            {
                                case "PoolName":
                                    poolInfo.poolName = xmlAttribute1.Value;
                                    break;
                                case "Prefab":
                                    poolInfo.path_prefab = xmlAttribute1.Value;
                                    poolInfo.prefab = Manager_Resources.Instance.Load<GameObject>(poolInfo.path_prefab);
                                    break;
                                case "PoolSize":
                                    poolInfo.poolSize = int.Parse(xmlAttribute1.Value);
                                    break;
                                case "FixedSize":
                                    poolInfo.fixedSize = bool.Parse(xmlAttribute1.Value);
                                    break;
                            }
                        }

                        poolInfos.Add(poolInfo);
                    }
                }
            }

            Init();
        }
    }


    #region 对象池
    /// <summary>
    /// 对象池
    /// </summary>
    public class ObjectPool
    {
        //池中可用对象的栈
        private Stack<PoolObject> availableObjStack = new Stack<PoolObject>();

        //是否是固定尺寸
        private bool fixedSize;

        //对象prefab
        private GameObject poolObjectPrefab;

        //池的大小
        private int poolSize;

        //池子的名称
        private string poolName;

        public ObjectPool(string poolName, GameObject poolObjectPrefab, int poolSize, bool fixedSize)
        {
            this.poolName = poolName;
            this.poolObjectPrefab = poolObjectPrefab;
            this.poolSize = poolSize;
            this.fixedSize = fixedSize;

            //填充池子
            for (int index = 0; index < poolSize; index++)
            {
                AddObjectToPool(NewObjectInstance());
            }
        }

        /// <summary>
        /// 将对象加入池，复杂度o(1)
        /// </summary>
        /// <param name="po"></param>
        private void AddObjectToPool(PoolObject po)
        {
            po.gameObject.SetActive(false);
            availableObjStack.Push(po);
        }

        /// <summary>
        /// 创建一个池子里的对象
        /// </summary>
        /// <returns></returns>
        private PoolObject NewObjectInstance()
        {
            GameObject go = (GameObject)UnityEngine.Object.Instantiate(poolObjectPrefab);
            go.transform.SetParent(Manager_ObjectPool.Instance.transform);
            PoolObject poolObject = go.GetComponent<PoolObject>();
            if (poolObject == null)
            {
                poolObject = go.AddComponent<PoolObject>();
            }
            poolObject.poolName = this.poolName;
            poolObject.isPooled = true;

            return poolObject;
        }

        /// <summary>
        /// 获取一个池子中的可用对象，如果没有可用的对象，则创建新对象
        /// </summary>
        public PoolObject NextAvailableObject(Vector3 position, Quaternion rotation)
        {
            PoolObject poolObject = null;
            if (availableObjStack.Count > 0)
            {
                poolObject = availableObjStack.Pop();
            }
            else if (fixedSize == false)
            {
                poolSize++;
                //Debug.Log(string.Format("Growing pool {0}. New size: {1}", poolName, poolSize));
                poolObject = NewObjectInstance();
            }
            else
            {
                //Debug.LogWarning("No object available & cannot grow pool: " + poolName);
            }

            //设置对象的行为
            GameObject result = null;
            if (poolObject != null)
            {
                poolObject.isPooled = false;
                result = poolObject.gameObject;
                result.SetActive(true);

                result.transform.position = position;
                result.transform.rotation = rotation;
            }
            return poolObject;
        }

        /// <summary>
        /// 将指定的对象返回池中
        /// </summary>
        /// <param name="poolObject"></param>
        public void ReturnObjectToPool(PoolObject poolObject)
        {
            if (poolName.Equals(poolObject.poolName))
            {
                if (poolObject.isPooled)
                {
                    //Debug.LogWarning(poolObject.gameObject.name + " is already in pool. Why are you trying to return it again? Check usage.");
                }
                else
                {
                    AddObjectToPool(poolObject);
                }
            }
            else
            {
                //Debug.LogError(string.Format("Trying to add object to incorrect pool {0} {1}", poolObject.poolName, poolName));
            }
        }
    }

    /// <summary>
    /// 池中对象
    /// </summary>
    public class PoolObject : MonoBehaviour
    {
        public string poolName;//对象所属池子的名字
        public bool isPooled;//是否已在池中（还未使用，待使用）
    }
    #endregion


    /// <summary>
    /// 对象池信息
    /// </summary>
    [Serializable]
    public class PoolInfo
    {
        //名称
        public string poolName;

        //prefab对象
        public string path_prefab;
        public GameObject prefab;

        //尺寸
        public int poolSize;

        //是否固定尺寸的池
        public bool fixedSize;
    }
}