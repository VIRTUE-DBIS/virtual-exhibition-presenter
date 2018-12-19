using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class AudioLoader : MonoBehaviour
    {

        private AudioSource audioSource;
        

        // Use this for initialization
        void Start()
        {
            audioSource =  gameObject.AddComponent<AudioSource>();
        }

        private IEnumerator LoadAudio(string url)
        {     
            using (WWW www = new WWW (url)){
                yield return www;

                if (www.isDone){
                    AudioClip audioClip = www.GetAudioClip(false, true) as AudioClip;
                    this.audioSource.clip = audioClip;
                    this.audioSource.volume = 0.2f;
                    this.audioSource.loop = true;
                    this.audioSource.Play();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Play()
        {
            audioSource.Play();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            audioSource.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void ReloadAudio(string url)
        {
            StartCoroutine(LoadAudio(url));
        }
    }
}