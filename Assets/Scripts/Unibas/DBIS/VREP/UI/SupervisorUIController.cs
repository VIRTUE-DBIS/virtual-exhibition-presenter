using UnityEngine;
using UnityEngine.UI;

namespace Unibas.DBIS.VREP.UI
{
    public class SupervisorUIController : MonoBehaviour
    {
        public Text timerInputLabel;
        public Text timerLabel;

        public void SetTimerDurationText(int duration)
        {
            timerInputLabel.text = duration.ToString();
        }

        public void SetTimerDuration()
        {
            // TODO: Set timer (and config) to the duration in the input field
            var duration = int.Parse(timerInputLabel.text);
            Debug.Log("Set the timer to " + duration + " seconds here!");
        }

        public void SetTimerLabel(int timeLeft)
        {
            var minutes = timeLeft / 60;
            var seconds = timeLeft % 60;
            timerLabel.text = minutes + ":" + seconds;
        }

        public void SetLeftController(bool on)
        {
            // TODO: Set controller visibility here
            Debug.Log("Set the left controller visibility to " + on + " here!");
        }
        
        public void SetRightController(bool on)
        {
            // TODO: Set controller visibility here
            Debug.Log("Set the right controller visibility to " + on + " here!");
        }
    }
}
