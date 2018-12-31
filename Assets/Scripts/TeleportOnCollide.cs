using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class TeleportOnCollide : MonoBehaviour
{
	public float CurrentFadeTime = 0.1f;
	
	private Player _player;

	private bool _disabled = false;
	
	private void Start()
	{
		_player = GetComponent<Player>();
		if (_player == null)
		{
			Debug.LogError("No Player found at this gameobject. Disable myself");
			_disabled = true;
		}
	}

	void OnCollisionEnter(Collision other)
	{
		Debug.Log("Collision Enter!");
		if (_disabled)
		{
			return;
		}
		
		TeleportDestinationProvider tdp = other.gameObject.GetComponent<TeleportDestinationProvider>();
		if (tdp == null)
		{
			return;
		}
		
		//Teleport.PlayerPre.Send( pointedAtTeleportMarker );

		SteamVR_Fade.Start( Color.clear, CurrentFadeTime );

		
		
		Vector3 playerFeetOffset = _player.trackingOriginTransform.position - _player.feetPositionGuess;
		_player.trackingOriginTransform.position = tdp.Destination + playerFeetOffset;
		
		

		//Teleport.Player.Send( pointedAtTeleportMarker );
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("Trigger Enter!");
	}
}
