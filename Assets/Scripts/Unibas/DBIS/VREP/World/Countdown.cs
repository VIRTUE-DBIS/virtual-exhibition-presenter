using System.Collections;
using UnityEngine;

namespace Unibas.DBIS.VREP.World
{
  public class Countdown : MonoBehaviour
  {
    public int initTime;
    private int _timeLeft;
    public TextMesh countdown;

    private void Start()
    {
      StartCoroutine(nameof(LoseTime));
      Time.timeScale = 1;
    }

    private void Update()
    {
      var min = (_timeLeft / 60).ToString().PadLeft(2, '0');
      var sec = (_timeLeft % 60).ToString().PadLeft(2, '0');
      countdown.text = min + ":" + sec;
    }

    public void Restart()
    {
      StopCoroutine(nameof(LoseTime));
      countdown.color = Color.white;
      StartCoroutine(nameof(LoseTime));
    }

    private IEnumerator LoseTime()
    {
      _timeLeft = initTime;
      GameObject.Find("VRCamera").GetComponent<Camera>().cullingMask = -1;
      while (_timeLeft > 0)
      {
        yield return new WaitForSeconds(1);
        _timeLeft--;
        if (_timeLeft > 30) continue;
        countdown.color = _timeLeft % 2 == 0 ? Color.red : Color.white;
      }

      GameObject.Find("VRCamera").GetComponent<Camera>().cullingMask = 0;
    }
  }
}