using System;
using UnityEngine;
namespace STools.Tool_Manager
{
    public class Manager_Scene : SingleMode<Manager_Scene>
    {
        int currentLevelIndex;

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="levelIndex"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public System.Collections.IEnumerator LoadScene(int levelIndex, Action<float> action)
        {
            this.currentLevelIndex = levelIndex;
            AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(levelIndex);
            asyncOperation.allowSceneActivation = false;
            while (!asyncOperation.isDone)
            {
                if (asyncOperation.progress >= 0.9f)
                {
                    action(1);
                    asyncOperation.allowSceneActivation = true;
                }
                yield return null;
            }
        }
    }
}