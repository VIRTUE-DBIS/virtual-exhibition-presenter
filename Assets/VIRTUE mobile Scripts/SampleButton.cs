using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SampleButton : MonoBehaviour
{
    
    public Button buttonComponent;
    public Text nameLabel;
    public Vector3 entry;
    
    private float OFFSET_X = 12;
    private float CONSTANT_Z = 1.8f;
    private float CONSTANT_Y = 1.3f;
    // public Image iconImage;

    // Use this for initialization
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(HandleClick);
        // buttonComponent.onClick.AddListener (HandleClick);
    }

    // Sets up the position and name of the teleport buttons
    public void Setup(int id, Vector3 pos)
    {
        nameLabel = GetComponentInChildren<Text>();
        nameLabel.text = "RoomID: " + id;
        CalculateTeleportPos(pos);

    }

    // The position after the teleport is the same as the previous 
    public void CalculateTeleportPos(Vector3 position)
    {
        entry = position;
        entry.x = position.x * OFFSET_X;
        entry.z = CONSTANT_Z;
        entry.y = CONSTANT_Y;
    }

    public void HandleClick()
    {
        Debug.Log(entry.ToString());
        PlayerMovement.Teleport(entry);
    }
}

public class roomTeleportationPos
{
     public int id;
     public Vector3 currentPos;
   public  roomTeleportationPos(int id, Vector3 currentPos)
    {
        this.id = id;
        this.currentPos = currentPos;
    }
   
}

