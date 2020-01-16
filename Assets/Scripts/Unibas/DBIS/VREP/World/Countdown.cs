using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Unibas.DBIS.VREP.World
{
    public class Countdown : MonoBehaviour {
        private int initTime;
        public int timeLeft;
        public TextMesh countdown;
        void Start ()
        {
            initTime = timeLeft;
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