using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class TrackerController : MonoBehaviour
{
    public GameObject trackerPrefab;
    
    void OnEnable() {
        SteamVR_Events.DeviceConnected.Listen(OnDeviceConnected);
        Debug.Log("Listening for connected devices...");
    }
 
    private void OnDeviceConnected(int index, bool connected) {
        if(connected) {
            if(OpenVR.System != null) {
                ETrackedDeviceClass deviceClass = OpenVR.System.GetTrackedDeviceClass((uint) index);
                if(deviceClass == ETrackedDeviceClass.GenericTracker) {
                    Debug.Log("Vive Tracker was connected at index: " + index);
                    GameObject tracker = Instantiate(trackerPrefab);
                    tracker.GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)index;
                    tracker.transform.SetParent(transform);
                }
            }
        }
    }
}
