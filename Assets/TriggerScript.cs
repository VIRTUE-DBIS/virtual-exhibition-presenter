using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int Id = 0;

    private void OnTriggerEnter(Collider other)
    {
        var cube = other.gameObject.GetComponent<TriggerScript>();
        if (cube != null)
        {
            Debug.Log("Other: "+cube.Id);
        }
    }
}
