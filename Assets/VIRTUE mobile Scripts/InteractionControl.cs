using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionControl : MonoBehaviour
{ 
    private static bool InteractionButton;
    private bool gyroEnabled;
    private Gyroscope gyro;

    private GameObject cameraContainer;
    private Quaternion rot;
    
    private Touch initTouch = new Touch();
        
    
    private float rotX = 0f;
    private float rotY = 0f;
    private Vector3 origRot;
    
    public float rotSpeed = 0.5f;
    public static float dir = -1;


    public float SpeedH = 5f;
    public float SpeedV = 5f;
 
    private float yaw = 0f;
    private float pitch = 0f;
    private float minPitch = -30f;
    private float maxPitch = 90f;

    private int pointerID;
    private float deltaX;
    private float deltaY;
    private Vector3 combine;
    
    
    // Start is called before the first frame update
    /*
     * At the start the interaction button is set as false, so the touch mode is used.
     * Also the gyroscope is set as true or false dependent on the EnableGyro() function.
     */
    void Start()
    { 
        origRot = transform.eulerAngles;
        rotX = origRot.x;
        rotY = origRot.y;
         
        cameraContainer = new GameObject("Camera Container");
        cameraContainer.transform.position = transform.position;
        transform.SetParent(cameraContainer.transform);
        gyroEnabled = EnableGyro();
        InteractionButton = false;
    }

    /*
     * Here the function first checks if the devices supports the gyroscope mode
     */
    private bool EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;

            cameraContainer.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
            rot = new Quaternion(0,0,1,0);
            return true;
        }
        return false;
    }


    // Update is called once per frame
    void Update()
    {
        // For devices which are not Android the mouse is used to rotate the camera
        if (Application.platform != RuntimePlatform.Android)
        {
            mouse();
        }
        
        // For Android devices there are two possibilities to rotate the camera
        else
        {
            /*
             * If an Android devices does not have a gyroscope sensor or the touch mode is on
             * the user can use the finger by swiping over the display 
             */ 
            if (!InteractionButton)
            {
                swiper();
            }
            /*
             * If the gyro mode is activated the user can rotate the tablet to rotate the camera
             */
            else
            {
                if (gyroEnabled)
                {
                    transform.localRotation = gyro.attitude * rot;
                }
                else
                {
                    InteractionButton = false;
                }
            }
        }
    }

    /*
     * This function is responsible for the swiping of the camera with the finger
     */
    void swiper()
    {
        foreach (Touch touch in Input.touches)
        {
             pointerID = touch.fingerId;

            if (EventSystem.current.IsPointerOverGameObject(pointerID))
            {
                return;
            }
            
            if (touch.phase == TouchPhase.Began)
            {
                initTouch = touch;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                //swiping
                deltaX = initTouch.position.x - touch.position.x;
                deltaY = initTouch.position.y - touch.position.y;
                pitch -= deltaY * Time.deltaTime * rotSpeed * dir;
                yaw += deltaX * Time.deltaTime * rotSpeed * dir;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
                combine.x = pitch;
                combine.y = yaw;
                transform.eulerAngles = combine;
                //transform.eulerAngles = new Vector3(pitch,yaw,0f);
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                initTouch = new Touch();
            }
        }
    }

    /*
     * This function is responsible for the rotation of the non Android devices
     * The camera follows the mouse position 
     */
    void mouse()
    {
        {
            yaw += Input.GetAxis("Mouse X") * SpeedH;
            pitch -= Input.GetAxis("Mouse Y") * SpeedV;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }
    }


    public static bool GetInteractionButton()
    {
        return InteractionButton;
    }

    public static void SetInteractionButton(bool flag)
    {
        InteractionButton = flag;
    }
}
