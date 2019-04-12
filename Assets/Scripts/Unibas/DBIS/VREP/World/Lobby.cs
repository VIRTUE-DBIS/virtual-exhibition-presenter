using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unibas.DBIS.DynamicModelling;
using Unibas.DBIS.DynamicModelling.Models;
using Unibas.DBIS.VREP.Puzzle;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;
using World;

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
                              new SteamVRTeleportButton.TeleportButtonModel(0.2f, .02f, 2f, TexturingUtility.LoadMaterialByName("none"),
                                  TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic"), hasPedestal:false),
                              "Wall");

        var cube = Utils.getNumberOfCubes(523, 345);
        var uvmap = PuzzleCubeFactory.CreateTiledMasterUVMap(523, 345, (int)cube.x, (int)cube.y);
        
        var test = PuzzleCubeFactory.createPuzzleCube(0, 1, uvmap, TexturingUtility.LoadMaterialByName("LimeBricks"));
        test.transform.parent = transform;
        test.transform.position = Vector3.zero;

        Debug.Log("cube");
        Debug.Log(cube);
    }

    public Texture2D tex;
    

    // Update is called once per frame
    void Update()
    {
        
    }
}