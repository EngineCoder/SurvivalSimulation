using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STools.Tool_Manager
{ 
    public class Manager_Atlas : SingleMode<Manager_Atlas>
    {
        /// <summary>
        /// 图集的集合
        /// </summary>
        private Dictionary<string, Object[]> dict_Atals = new Dictionary<string, Object[]>();

        /// <summary>
        /// 加载一个精灵
        /// </summary>
        /// <param name="path_Atlas"></param>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public Sprite LoadSpriteFromAtlas(string path_Atlas, string spriteName)
        {
            Sprite sprite = null;

            Object[] sprites = null;

            if (dict_Atals.ContainsKey(path_Atlas))//如果已经加载过该图集
            {
                sprites = dict_Atals[path_Atlas];//获取图集中的所有精灵

                for (int i = 0; i < sprites.Length; i++)//判断图集中是否有该精灵
                {
                    if (sprites[i].GetType() == typeof(UnityEngine.Sprite))
                    {
                        if (sprites[i].name == spriteName)
                        {
                            sprite = (Sprite)sprites[i];
                            return sprite;
                        }
                    }
                }
                Debug.LogWarning("Sprite Name:" + spriteName + "，在图集中找不到");
                return null;
            }
            else
            {
                sprites = Resources.LoadAll<Sprite>(path_Atlas);
                if (sprites.Length > 0)
                {
                    dict_Atals.Add(path_Atlas, sprites);
                    for (int i = 0; i < sprites.Length; i++)//判断图集中是否有该精灵
                    {
                        if (sprites[i].GetType() == typeof(UnityEngine.Sprite))
                        {
                            if (sprites[i].name == spriteName)
                            {
                                sprite = (Sprite)sprites[i];
                                return sprite;
                            }
                        }
                    }
                    Debug.LogWarning("Sprite Name:" + spriteName + "，在图集中找不到");
                    return null;
                }
                else
                {
                    Debug.LogWarning("图集中不含任何精灵");
                    return null;
                }
            }
        }

        /// <summary>
        /// 从字典中移除某个图集
        /// </summary>
        /// <param name="path_Atlas"></param>
        public void RemoveAtalsFromDict_Atals(string path_Atlas)
        {
            if (dict_Atals.ContainsKey(path_Atlas))
            {
                dict_Atals.Remove(path_Atlas);
            }
        }
    }

}
