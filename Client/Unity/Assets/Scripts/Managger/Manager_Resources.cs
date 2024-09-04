using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STools.Tool_Manager
{
    public class Manager_Resources : SingleMode<Manager_Resources>
    {
        public T Load<T>(string filePath) where T : Object
        {
            return Resources.Load<T>(filePath);
        }

        public T Load<T>(object resourceType, object enumTypeValue) where T : Object
        {
            string enumResType = resourceType.ToString();
            string enumTypeName = enumTypeValue.GetType().Name;
            string filePath = enumResType + "/" + enumTypeName + "/" + enumTypeValue.ToString();
            return Resources.Load<T>(filePath);
        }
    }

    #region ResourcesType

    /// <summary>
    /// Resources Type
    /// </summary>
    public enum ResourcesType : byte
    {
        Music = 0,
        Prefabs,
        TextAsset
    }

    /// <summary>
    /// ResType_Prefabs
    /// </summary>
    public class ResType_Prefabs
    {
        public enum AudioSource : byte
        {
            AudioSource = 0,
        }

        public enum Panel : byte
        {
            Panel0 = 0,
            Panel1
        }
    }

    /// <summary>
    /// ResType_Music
    /// </summary>
    public class ResType_Music
    {
        /// <summary>
        /// 战斗音效
        /// </summary>
        public enum Battle : byte
        {

        }

        /// <summary>
        /// 提示音
        /// </summary>
        public enum Beep : byte
        {
            JoinIn
        }

        /// <summary>
        /// 背景音乐
        /// </summary>
        public enum BGM : byte
        {
            BGM_00 = 0,
            BGM_01,
            BGM_02,
            BGM_03,
        }

        /// <summary>
        /// 按钮
        /// </summary>
        public enum Button : byte
        {
            Create_01 = 0,
            Login_01,
            MaoPao,
            ToggleOF_01,
            ToggleON_01,
        }

        /// <summary>
        /// 环境
        /// </summary>
        public enum Environment : byte
        {
            Thunder_01,
            Thtnder_02,
            WaterDrops_01,
            WaterDrops_02,
            Wind_01
        }

        /// <summary>
        /// 心态
        /// </summary>
        public enum Mentality : byte
        {
            JianDing
        }

        /// <summary>
        /// 情绪
        /// </summary>
        public enum Mood : byte
        {

        }

        /// <summary>
        /// 结果
        /// </summary>
        public enum Result : byte
        {

        }
    }

    /// <summary>
    /// ResType_TextAsset
    /// </summary>
    public class ResType_TextAsset
    {
        /// <summary>
        /// Json文件
        /// </summary>
        public enum Json : byte
        {

        }
        /// <summary>
        /// Xml文件
        /// </summary>
        public enum Xml : byte
        {
            PoolInfo,
        }

        /// <summary>
        /// 表格文件
        /// </summary>
        public enum Xlsx : byte
        {

        }
    }

    #endregion

    /// <summary>
    /// Pool_Audio
    /// </summary>
    public class Pool_Audio
    {
        #region Private Attributes
        private GameObject prefab_Audio;
        private List<GameObject> pool_Audio;
        #endregion

        #region Methods Construction
        /// <summary>
        /// Construction Method Initial Audio Pool.
        /// </summary>
        /// <param name="prefab_Audio"></param>
        /// <param name="initial_Capacity"></param>
        /// <param name="parentTransform"></param>
        public Pool_Audio(GameObject prefab_Audio, int initial_Capacity, Transform parentTransform)
        {
            this.prefab_Audio = prefab_Audio;

            this.pool_Audio = new List<GameObject>();

            for (int i = 0; i < initial_Capacity; i++)
            {
                Initial_PoolAudio(parentTransform);
            }
        }
        #endregion

        #region Methods Private
        /// <summary>
        /// Initial Pool Audio By Prefab_Audio
        /// </summary>
        /// <param name="parentTransform"></param>
        private void Initial_PoolAudio(Transform parentTransform)
        {
            GameObject instance = (GameObject)Object.Instantiate(prefab_Audio);
            instance.transform.SetParent(parentTransform);
            instance.SetActive(false);

            pool_Audio.Add(instance);
        }
        #endregion

        #region Methods Public
        /// <summary>
        /// Get AudioUnit From Pool_Audio
        /// </summary>
        /// <returns></returns>
        public GameObject GetAudioUnit(Transform parentTranform)
        {
            if (pool_Audio.Count == 0)
            {
                Initial_PoolAudio(parentTranform);
            }
            GameObject audioUnit = pool_Audio[0];
            pool_Audio.RemoveAt(0);
            audioUnit.SetActive(true);

            return audioUnit;
        }

        /// <summary>
        /// Put The AuidoUnit in the Pool_Audio
        /// </summary>
        /// <param name="instance"></param>
        public void PutAudioUnit(GameObject unit)
        {
            unit.SetActive(false);
            pool_Audio.Add(unit);
        }
        #endregion
    }
}