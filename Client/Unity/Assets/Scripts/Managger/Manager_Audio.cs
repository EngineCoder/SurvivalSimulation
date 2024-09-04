using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Exception = System.Exception;
using Object = UnityEngine.Object;

namespace STools.Tool_Manager
{

    /// <summary>
    /// Audio Manager
    /// </summary>
    public class Manager_Audio : SingleMode<Manager_Audio>
    {
        #region Public Attributes
        public int init_PoolCapacity = 2;
        public List<AudioClip> audioClips = new List<AudioClip>();

        private bool isMute = false;
        public bool IsMute
        {
            set
            {
                isMute = value;
                if (isMute)
                {
                    tempVolume = AudioListener.volume;
                    AudioListener.volume = 0;//静音设置，音量为0
                }
                else
                {
                    AudioListener.volume = tempVolume;
                }
            }
            private get { return isMute; }
        }

        private float volumeScale = 1;
        public float VolumeScale
        {
            set
            {
                volumeScale = Mathf.Clamp01(value);
                if (!IsMute)
                {
                    AudioListener.volume = volumeScale;
                }
            }
            private get { return volumeScale; }
        }
        #endregion

        /// <summary>
        /// Store Volume Before Mute.
        /// </summary>
        [HideInInspector]
        public float tempVolume = 0;

        private Dictionary<string, AudioClip> audioDic = new Dictionary<string, AudioClip>();

        #region Audio Play Methods

        /// <summary>
        /// Play BG Music By Music Type
        /// </summary>
        /// <param name="enumResourceType">ResourcesType.Music</param>
        /// <param name="enumTypeValue">ResType_Music.BGM.BGM_00</param>
        /// <param name="audioSourcePosition">Camera.main.transform.position</param>
        /// <param name="restart">false</param>
        /// <param name="isLoop">true</param>
        /// <param name="volume">1</param>
        /// <returns></returns>
        private AudioSource Base_PlayBGMAudioByMusicType(object enumResourceType, object enumTypeValue, Vector3 audioSourcePosition, bool restart = false, bool isLoop = true, float volume = 1)
        {
            if (IsMute)//静音，音量为0时
            {
                return null;
            }

            GameObject audiGo = null;//IsExistBGMAudioSource(enumTypeValue.GetType().Name);

            if (audiGo)//如果已经在播放
            {
                AudioSource audioSource = audiGo.GetComponent<AudioSource>();
                if (restart)//重写播放新的
                {
                    AudioClip audioClip = Manager_Resources.Instance.Load<AudioClip>(enumResourceType, enumTypeValue);
                    if (audioClip != null)
                    {
                        audiGo.name = audioClip.name;
                        audioSource.clip = audioClip;
                        audioSource.volume = volume;
                        audioSource.loop = isLoop;

                        audioSource.Play();

                        if (!isLoop)//不是循环时，播放完，回收放到池中
                        {
                            //TO DO
                            //StartCoroutine(RecycleAudioSource(audioSource, audioClip.length));
                        }
                    }
                }
                return audioSource;
            }
            else
            {
                GameObject audioGo = Manager_ObjectPool.Instance.GetObjectFromPool("AudioSource", Camera.main.transform.position, Quaternion.identity).gameObject;
                audioGo.transform.position = audioSourcePosition;
                AudioSource audioSource = audioGo.GetComponent<AudioSource>();
                AudioClip audioClip = Manager_Resources.Instance.Load<AudioClip>(enumResourceType, enumTypeValue);
                if (audioClip != null)
                {
                    audioGo.name = audioClip.name;
                    audioSource.clip = audioClip;
                    audioSource.volume = volume;
                    audioSource.loop = isLoop;

                    audioSource.Play();

                    if (!isLoop)//不是循环时，播放完一次，回收
                    {
                        //TO DO
                        //StartCoroutine(RecycleAudioSource(audioSource, audioClip.length));
                    }
                }
                return audioSource;
            }
        }


        public AudioSource PlayBGMAudioByMusicType<T>(T enumTypeMusicValue, Vector3 position, bool restart = false, bool isLoop = true)
        {
            return Base_PlayBGMAudioByMusicType(ResourcesType.Music, enumTypeMusicValue, position, restart, isLoop);
        }



        /// <summary>
        /// Play Audio By Music Type
        /// </summary>
        /// <param name="enumTypeValue"></param>
        /// <param name="audioSourcePosition"></param>
        /// <param name="is3D"></param>
        /// <param name="isLoop"></param>
        /// <param name="volume"></param>
        private void Base_PlayAudioByMusicType(object enumResourceType, object enumTypeValue, Vector3 audioSourcePosition, bool is3D = true, bool isLoop = false, float volume = 1)
        {
            if (IsMute)
            {
                return;
            }

            AudioClip audioClip = Manager_Resources.Instance.Load<AudioClip>(enumResourceType, enumTypeValue);

            if (is3D)
            {
                AudioSource.PlayClipAtPoint(audioClip, audioSourcePosition, volume);
            }
            else
            {
                GameObject go = Manager_ObjectPool.Instance.GetObjectFromPool("AudioSource", Camera.main.transform.position, Quaternion.identity).gameObject;
                AudioSource audioSource = go.GetComponent<AudioSource>();
                audioSource.PlayOneShot(audioClip, volume);
                if (!isLoop)
                {
                    //TO DO
                    //StartCoroutine(RecycleAudioSource(audioSource, audioClip.length));
                }
            }
        }
        public void PlayAudioByMusicType<T>(T enumTypeMusicValue, Vector3 position, bool is3D = true, bool isLoop = false, float volume = 1)
        {
            Base_PlayAudioByMusicType(ResourcesType.Music, enumTypeMusicValue, position, is3D, isLoop, volume);
        }



        /// <summary>
        /// Play Audio By AudioClip Name
        /// </summary>
        /// <param name="audioSourcePosition"></param>
        /// <param name="clipName"></param>
        /// <param name="is2D"></param>
        /// <param name="isLoop"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public AudioSource PlayAudioByClipName(Vector3 audioSourcePosition, string clipName, bool is2D = true, bool isLoop = false, float volume = 1)
        {
            if (IsMute)//静音，音量为0时
            {
                return null;
            }
            AudioClip audioClip;
            if (audioDic.TryGetValue(clipName, out audioClip))
            {
                if (is2D)
                {
                    GameObject audioGo = Manager_ObjectPool.Instance.GetObjectFromPool("AudioSource", Camera.main.transform.position, Quaternion.identity).gameObject;

                    audioGo.transform.position = audioSourcePosition;
                    audioGo.name = clipName;

                    AudioSource audioSource = audioGo.GetComponent<AudioSource>();
                    audioSource.clip = audioClip;
                    audioSource.PlayOneShot(audioClip, volume);
                    if (!isLoop)//不是循环时，播放完一次，回收
                    {
                        //TO DO
                        //   StartCoroutine(RecycleAudioSource(audioSource, audioClip.length));
                    }
                    return audioSource;
                }
                else
                {
                    AudioSource.PlayClipAtPoint(audioClip, audioSourcePosition, volume);
                }
            }
            return null;
        }


        /// <summary>
        /// Play Audio By AudioSource
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="audioSourcePosition"></param>
        /// <param name="clipName"></param>
        /// <param name="is2D"></param>
        /// <param name="isLoop"></param>
        /// <param name="volume"></param>
        public void PlayAudioByAudioSource(AudioSource audioSource, Vector3 audioSourcePosition, string clipName, bool is2D = true, bool isLoop = false, float volume = 1)
        {
            if (isMute)
            {
                return;
            }
            AudioClip audioClip;
            if (audioDic.TryGetValue(clipName, out audioClip))
            {
                if (null == audioSource)
                {
                    if (is2D)
                    {
                        GameObject audioGo = Manager_ObjectPool.Instance.GetObjectFromPool("AudioSource", Camera.main.transform.position, Quaternion.identity).gameObject;
                        AudioSource sourceAudio = audioGo.GetComponent<AudioSource>();
                        audioGo.name = clipName;
                        sourceAudio.loop = isLoop;
                        sourceAudio.PlayOneShot(audioClip, volume);
                        if (!isLoop)
                        {
                            //TO DO
                            //StartCoroutine(RecycleAudioSource(audioSource, audioClip.length));
                        }
                    }
                    else
                    {
                        AudioSource.PlayClipAtPoint(audioClip, audioSourcePosition, volume);
                    }

                }
                else
                {
                    if (is2D)
                    {
                        audioSource.clip = audioClip;
                        audioSource.loop = isLoop;
                        audioSource.PlayOneShot(audioClip, volume);

                        if (!isLoop)
                        {
                            //TO DO
                            // StartCoroutine(RecycleAudioSource(audioSource, audioClip.length));
                        }

                    }
                    else
                    {
                        AudioSource.PlayClipAtPoint(audioClip, audioSourcePosition, volume);
                    }
                }
            }
        }


        public void PauseAudio(AudioSource audioSource)
        {
            audioSource.Pause();
        }
        public void ResumeAudio(AudioSource audioSource)
        {
            //audioSource.Pause();
        }
        public void StopAudio(AudioSource audioSource)
        {
            audioSource.Stop();
        }

        #endregion
    }
}