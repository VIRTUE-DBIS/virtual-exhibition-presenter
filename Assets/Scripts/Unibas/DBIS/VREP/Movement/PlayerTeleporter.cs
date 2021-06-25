using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Unibas.DBIS.VREP.Movement
{
  public class PlayerTeleporter : MonoBehaviour
  {
    public Vector3 destination = Vector3.zero;


    public float minDistance = 1f;

    public void TeleportPlayer()
    {
      SteamVR_Fade.Start(Color.clear, 0.2f);

      var pgo = GameObject.FindWithTag("Player");
      if (pgo == null)
      {
        Debug.LogWarning("No player found!");
      }

      var player = pgo.GetComponent<Player>();

      if (player == null)
      {
        Debug.LogWarning("No SteamVR Player attached!");
      }

      var playerFeetOffset = player.trackingOriginTransform.position - player.feetPositionGuess;
      player.trackingOriginTransform.position = destination + playerFeetOffset;
    }
  }
}