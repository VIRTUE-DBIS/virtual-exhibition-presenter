using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DefaultNamespace.VREM.Model;

[System.Serializable]
public class Item
{
    public string itemName;
    // public Sprite icon;
    public float id;
}

public class TeleportButtons : MonoBehaviour {

    public List<Item> itemList;
    public Transform contentPanel;
    public SimpleObjectPool buttonObjectPool;
    public static Exhibition ex;
    private bool onetime = false;
    private DefaultNamespace.VREM.Model.Room []rooms;
    
    // Use this for initialization
    void Start () 
    {
        //RefreshDisplay ();
    }
    
    public static void setExhibition(Exhibition exhibition)
    {
        ex = exhibition;
    }

    void RefreshDisplay()
    {
        AddButtons ();
    }

    public void Update()
    {
        if (!onetime)
        {
            if (!ex.Equals(null))
            {
                rooms = ex.rooms;
                RefreshDisplay();
                onetime = true;
            }
        }
        {
            
        }
    }

    // Adds the teleport buttons to the scroll bar 
    private void AddButtons()
    {
        for (int i = 0; i < rooms.Length; i++) 
        {
            GameObject newButton = buttonObjectPool.GetObject();
            newButton.transform.SetParent(contentPanel);

            SampleButton sampleButton = newButton.GetComponent<SampleButton>();
            sampleButton.Setup(i,rooms[i].position);
        }
    }
}