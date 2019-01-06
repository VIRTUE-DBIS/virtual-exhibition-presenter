using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unibas.DBIS.DynamicModelling;
using Unibas.DBIS.DynamicModelling.Models;
using Unibas.DBIS.VREP.World;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class Lobby : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

        var tp = SteamVRTeleportButton.Create(gameObject, new Vector3(0, 0, 4.5f), Vector3.zero,
            new SteamVRTeleportButton.TeleportButtonModel(0.1f, .02f, 1f, TexturingUtility.LoadMaterialByName("NWood"),
                TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic")),
            "Text");
    }
    
    

    // Update is called once per frame
    void Update()
    {
    }
}