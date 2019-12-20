using DefaultNamespace;
using Unibas.DBIS.VREP.Core;
using UnityEngine;
using World;

public class Lobby : MonoBehaviour
{
    private SteamVRTeleportButton text;
    private SteamVRTeleportButton next;
    private SteamVRTeleportButton wall;
    
    // Use this for initialization
    void Start()
    {

        text = SteamVRTeleportButton.Create(gameObject, new Vector3(0, 0, 4.5f), Vector3.zero,
            new SteamVRTeleportButton.TeleportButtonModel(0.1f, .02f, 1f, TexturingUtility.LoadMaterialByName("none"),
                TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic")),
            "Text");
        next = SteamVRTeleportButton.Create(gameObject, new Vector3(.5f, 0, 4.5f), Vector3.zero,
            new SteamVRTeleportButton.TeleportButtonModel(0.1f, .02f, 1f, TexturingUtility.LoadMaterialByName("NWood"),
                TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic")),
            Resources.Load<Sprite>("Sprites/UI/chevron-right"));
        wall = SteamVRTeleportButton.Create(gameObject, new Vector3(0, 1.5f, 4.98f), Vector3.zero,
            new SteamVRTeleportButton.TeleportButtonModel(0.2f, .02f, 2f, TexturingUtility.LoadMaterialByName("none"),
                TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic"), hasPedestal:false),
            "Wall");
    }

    public void activateRoomTrigger(ExhibitionManager manager)
    {
        var room = manager.GetRoomByIndex(0);
        if (room != null)
        {
            text.OnTeleportEnd = room.OnRoomEnter;
            next.OnTeleportEnd = room.OnRoomEnter;
            wall.OnTeleportEnd = room.OnRoomEnter;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}