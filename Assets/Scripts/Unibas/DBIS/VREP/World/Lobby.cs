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
            new SteamVRTeleportButton.TeleportButtonModel(0.1f, .02f, 1f, TexturingUtility.LoadMaterialByName("none"),
                TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic")),
            "Text");
        var tp2 = SteamVRTeleportButton.Create(gameObject, new Vector3(.5f, 0, 4.5f), Vector3.zero,
            new SteamVRTeleportButton.TeleportButtonModel(0.1f, .02f, 1f, TexturingUtility.LoadMaterialByName("NWood"),
                TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic")),
            Resources.Load<Sprite>("Sprites/UI/chevron-right"));
        var tp3 = SteamVRTeleportButton.Create(gameObject, new Vector3(0, 1.5f, 4.98f), Vector3.zero,
                              new SteamVRTeleportButton.TeleportButtonModel(0.1f, .02f, 1f, TexturingUtility.LoadMaterialByName("none"),
                                  TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic"), hasPedestal:false),
                              "Wall");
    }
    
    

    // Update is called once per frame
    void Update()
    {
        
    }
}