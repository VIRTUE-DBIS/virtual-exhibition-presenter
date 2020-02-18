using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoyStickScript : MonoBehaviour,IDragHandler,IPointerUpHandler,IPointerDownHandler
{
    [Header("Tweaks")] [SerializeField] private float joystickVisualDistance = 50;
    [Header("Logic")] private Image joystick;
    private Image container;
    public static Vector3 direction;
    public GameObject InfoWindow;

    // Direction of the movement 
    public Vector3 Direction
    {
        get { return direction; }
    }

    // Start is called before the first frame update
    void Start()
    {
        /* 
         * For Android devices the joy stick gets activated and is visible
         * Because the touch mode is only created for the Android devices the info screen only appears here for one second
         */
        if (Application.platform == RuntimePlatform.Android)
        {
            var imgs = GetComponentsInChildren<Image>();
            container = imgs[0];
            joystick = imgs[1];
            StartCoroutine(RemoveAfterSeconds(1, InfoWindow));
        }
        
        // For devices which are not running on Android the joy stick is not activated 
        else
        {
            GameObject joy = GameObject.FindGameObjectWithTag("joy");
            joy.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    /*
     * This event updates the position of the x and y axis of the player by using the joy stick
     */
    public void OnDrag(PointerEventData ped)
    {
        Vector2 pos = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(container.rectTransform, ped.position,
            ped.pressEventCamera, out pos))
        {
            pos.x = (pos.x / container.rectTransform.sizeDelta.x);
            pos.y = (pos.y / container.rectTransform.sizeDelta.y);
            
            Vector2 refpivot = new Vector2(0.5f,0.5f);
            Vector2 p = container.rectTransform.pivot;
            pos.x += p.x - 0.5f;
            pos.y += p.y - 0.5f;

            float x = Mathf.Clamp(pos.x, -1, 1);
            float y = Mathf.Clamp(pos.y, -1, 1);
            
            direction = new Vector3(x,0,y);
            Debug.Log(direction);
            
            joystick.rectTransform.anchoredPosition  = new Vector3(x*joystickVisualDistance,y*joystickVisualDistance);

        }
    }

    /*
     * When the joy stick is released the player position stays at the last position 
     */
    public void OnPointerUp(PointerEventData eventData)
    {
        direction = default(Vector3);
        joystick.rectTransform.anchoredPosition = default(Vector3);
    }

    /*
     * When the button is pressed the OnDrag() event starts
     */
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }
    
    /*
     * After one second the info screen which shows the area for the touch mode disappears
     */
    IEnumerator RemoveAfterSeconds(int seconds, GameObject obj)
    {
        obj.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }
}
