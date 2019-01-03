using System.Collections;
using System.Collections.Generic;
using Unibas.DBIS.DynamicModelling;
using Unibas.DBIS.DynamicModelling.Models;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class Lobby : MonoBehaviour
{
    private static ComplexCuboidModel GenerateButtonModel(float size, float border, float height)
    {
        ComplexCuboidModel model = new ComplexCuboidModel();
        // TODO Add material somehow
        model.Add(Vector3.zero, new CuboidModel(size, size, height));
        model.Add(new Vector3(border, border, -height), new CuboidModel(size - 2 * border, size - 2 * border, height));
        return model;
    }

    public static GameObject CreateTeleportButtonModel(Vector3 position, Vector3 destination, float size, float border)
    {
        var modelData = GenerateButtonModel(size, border, border / 2f);
        GameObject buttonObj = ModelFactory.CreateModel(modelData);
        TeleportButton tpBtn = buttonObj.AddComponent<TeleportButton>();
        tpBtn.Destination = destination;
        BoxCollider col = buttonObj.AddComponent<BoxCollider>();
        buttonObj.AddComponent<Button>();
        var hand = new CustomEvents.UnityEventHand();
        hand.AddListener(h =>
        {
            tpBtn.TeleportPlayer();
        });
        buttonObj.AddComponent<UIElement>().onHandClick = hand;
        buttonObj.transform.position = position;
        return buttonObj;
    }

    // Use this for initialization
    void Start()
    {
        float size = .1f, border = 0.01f, height = .02f;
        GameObject buttonObj = ModelFactory.CreateModel(GenerateButtonModel(size, border, height));
        TeleportButton tpBtn = buttonObj.AddComponent<TeleportButton>();
        BoxCollider col = buttonObj.AddComponent<BoxCollider>();
        col.size = new Vector3(size, size, height);
        col.center = new Vector3(size / 2f, size / 2f, -height);
        buttonObj.AddComponent<Button>();
        var hand = new CustomEvents.UnityEventHand();
        hand.AddListener(h =>
                     {
                         Debug.Log("CLICKED!");
                         tpBtn.TeleportPlayer();
                     });
        buttonObj.AddComponent<UIElement>().onHandClick = hand;
            
        buttonObj.transform.parent = transform;
        buttonObj.transform.localPosition = new Vector3(0, 1.5f, 4.9f);
    }
    
    

    // Update is called once per frame
    void Update()
    {
    }
}