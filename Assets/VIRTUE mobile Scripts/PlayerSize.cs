using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSize : MonoBehaviour
{
   
    private float sliderValue;
    public GameObject FirstPerson;
    public Slider slider;
    
    // Start is called before the first frame update
    void Start()
    {
       
    }
    
    // Update is called once per frame
    void Update()
    {
       
    }

    /*
     * Updates the player height with the slider value by using the slider in the settings menu
     * :param sliderValue : value of the slider
     * :param temp : vector which shows the player position
     */
    public void updateHeight()
    {
        sliderValue = slider.value;
        Vector3 temp = FirstPerson.transform.position;
        temp.y = sliderValue;
        FirstPerson.transform.position = temp;
    }
}
