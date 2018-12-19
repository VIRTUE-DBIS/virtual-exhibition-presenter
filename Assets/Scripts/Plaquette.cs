using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plaquette : MonoBehaviour
{
    public Text text;

    public Font font;

    // Use this for initialization
    void Start()
    {
        if (font != null)
        {
            text.font = font;
        }
    }
}