using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModeSwitchButton : MonoBehaviour {
    
    private static bool InteractionButton;
    public Sprite first;
    public Sprite second;
    public Button T_Button;
    public GameObject InfoWindow;
    private Text T_Text;

    private void Start()
    {
        // In the beginning the info window is inactive
        InfoWindow.gameObject.SetActive(false);
    }

    /*
     * When the interaction button is clicked the modes switch between the gyro mode or the touch mode
     * The default mode is the touch mode because not every device supports the gyroscope, so no text shows the mode
     */
    public void OnClickInteractionButton()
    {
        InteractionButton = InteractionControl.GetInteractionButton();
        T_Button = GetComponentInChildren<Button>();
        T_Text = T_Button.GetComponentInChildren<Text>();

        /* 
         * When the interaction button is pressed the gyro mode gets activated
         * Also the text and sprite change
         */
        if (!InteractionButton)
        {
            InteractionControl.SetInteractionButton(true);
            T_Text.text = "GM";
            T_Text.transform.position = new Vector3(1998, 1056, 0);
            T_Button.image.overrideSprite = first;
        }
        
        /*
         * With another click on the interaction button the mode switches back to touch mode
         * The sprite changes again but now with the text which shows that the touch mode is activated 
         */
        else
        {
            InteractionControl.SetInteractionButton(false);
            T_Text.text = "TM";
            T_Button.image.overrideSprite = second;
            T_Text.transform.position = new Vector3(1919, 1056, 0);
        }
    }

    // By switching back to the touch mode the info screen which shows the area to use for it appears for one second 
    public void YourFunction()
    {
        if (InteractionButton )
        {
            StartCoroutine(RemoveAfterSeconds(1, InfoWindow));
        }
    }
 
    // Function which disables the info screen after one second
    IEnumerator RemoveAfterSeconds(int seconds, GameObject obj)
    {
        obj.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }
    
}
