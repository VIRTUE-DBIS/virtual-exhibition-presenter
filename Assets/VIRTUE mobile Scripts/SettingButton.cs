using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour
{
    public Button settingsButton;
    public Sprite firstSetting;
    public Sprite secondSetting;
    private int counter = 0;
    public Image menu;
    
    // Start is called before the first frame update
    void Start()
    {
        // In the beginning the settings menu is not visible
        menu.gameObject.SetActive(false);
    }

    /*
     * This function makes the settings menu appear and disappear with clicks on the button, as well
     * as changing the button sprite
     * : param counter : with every second click on the button the settings window disappears
     * : param firstSetting and secondSetting : sprites of the button which change with every click
     */
    public void OnClickSettingButton()
    {
        counter++;
        if (counter % 2 == 1)
        {
            menu.gameObject.SetActive(true);
            settingsButton.image.overrideSprite = secondSetting;
        }

        if (counter % 2 == 0)
        {
            menu.gameObject.SetActive(false);
            settingsButton.image.overrideSprite = firstSetting;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
