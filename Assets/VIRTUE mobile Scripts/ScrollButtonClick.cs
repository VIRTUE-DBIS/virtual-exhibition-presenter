using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollButtonClick : MonoBehaviour
{
    private int counter = 0;
    public Button scrollButton;
    public Sprite firstScroll;
    public Sprite secondScroll;
    public GameObject scrollBar;
    
    // Start is called before the first frame update
    void Start()
    {
        // In the beginning the scroll bar for the teleportation is not visible
        scrollBar.gameObject.SetActive(false);
    }
    
    /*
     * this function makes the scroll bar appear and disappear with clicks on the button, as well
     * as changing the button sprite
     * : param counter : with every second click on the button the settings window disappears
     * : param firstScroll and secondScroll : sprites of the button which change with every click
     */
    public void OnClickScrollButton()
    {
        counter++;
        if (counter % 2 == 1)
        {
            scrollBar.gameObject.SetActive(true);
            scrollButton.image.overrideSprite = secondScroll;
        }

        if (counter % 2 == 0)
        {
            scrollBar.gameObject.SetActive(false);
            scrollButton.image.overrideSprite = firstScroll;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
