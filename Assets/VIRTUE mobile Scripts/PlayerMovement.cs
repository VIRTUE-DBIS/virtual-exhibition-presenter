using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public static CharacterController control;
    float x;
    float z;
    private Vector3 move;
    public float speed = 5.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        control = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // For devices which are not Android the arrow keys are used to move
        if (Application.platform != RuntimePlatform.Android)
        {
            x = Input.GetAxis("Horizontal") * speed;
            z = Input.GetAxis("Vertical") * speed;
            
            // By pressing the esc button the program closes
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }
        }
        // For Android devices the joystick is used
        else
        {
             x = JoyStickScript.direction.x;
             z = JoyStickScript.direction.z;
        }
        
        // The move vector gets updated by the speed and direction
        move = transform.right * x + transform.forward * z;
        move.y = 0;
        control.Move(move * speed * Time.deltaTime);
    }

    public static void Teleport(Vector3 pos)
    {
        control.enabled = false;
        control.transform.position = pos;
        control.enabled = true;
    }
}

