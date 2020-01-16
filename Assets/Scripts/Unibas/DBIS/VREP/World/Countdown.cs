using System.Collections;
using UnityEngine;

namespace Unibas.DBIS.VREP.World
{
    public class Countdown : MonoBehaviour {
        public int initTime;
        private int timeLeft;
        public TextMesh countdown;
        void Start ()
        {
            StartCoroutine("LoseTime");
            Time.timeScale = 1;
        }
        void Update ()
        {
            var min = (timeLeft / 60).ToString().PadLeft(2,'0');
            var sec = (timeLeft % 60).ToString().PadLeft(2,'0');
            countdown.text = (min + ":" + sec);
        }

        public void Restart()
        {
            StopCoroutine("LoseTime");
            StartCoroutine("LoseTime");
        }
        
        IEnumerator LoseTime()
        {
            timeLeft = initTime;
            GameObject.Find("VRCamera").GetComponent<Camera>().cullingMask = -1;
            while (true) {
                yield return new WaitForSeconds (1);
                timeLeft--;
                if (timeLeft <= 30)
                {
                    if (timeLeft % 2 == 0)
                    {
                        countdown.color = Color.red;
                        if (timeLeft == 0)
                        {
                            StopCoroutine("LoseTime");
                            GameObject.Find("VRCamera").GetComponent<Camera>().cullingMask = 0;
                        }
                    }
                    else
                    {
                        countdown.color = Color.white;
                    }
                }
            }
        }
    }
}