using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using UnityEngine;
using Valve.VR;

namespace Unibas.DBIS.VREP.Covis
{
    public class TrackerController : MonoBehaviour
    {
        public string[] trackerPrefabNames;

        private int trackerPrefabIndex = 0;

        void OnEnable()
        {
            SteamVR_Events.DeviceConnected.Listen(OnDeviceConnected);
            Debug.Log("Listening for connected devices...");
        }

        private void OnDeviceConnected(int index, bool connected)
        {
            if (connected)
            {
                if (OpenVR.System != null)
                {
                    ETrackedDeviceClass deviceClass = OpenVR.System.GetTrackedDeviceClass((uint) index);
                    if (deviceClass == ETrackedDeviceClass.GenericTracker)
                    {
                        Debug.Log("Vive Tracker was connected at index: " + index);
                        if (trackerPrefabIndex >= trackerPrefabNames.Length)
                        {
                            Debug.LogError("Not enough distinct tracker prefab names given! Looping to beginning.");
                        }
                        InstantiateTracker(trackerPrefabNames[trackerPrefabIndex % trackerPrefabNames.Length], index);
                        trackerPrefabIndex++;
                    }
                }
            }
        }

        private void InstantiateTracker(string model, int index)
        {
            var instance = (GameObject)Instantiate(Resources.Load("Prefabs/Syncables/" + model), transform, true);
            SteamVR_TrackedObject trackedObjectScript = instance.AddComponent<SteamVR_TrackedObject>();
            trackedObjectScript.index = (SteamVR_TrackedObject.EIndex) index;

            var syncables = new Dictionary<string, Syncable>();
            var syncableComponent = instance.AddComponent<Syncable>();
            var containerComp = instance.AddComponent<SyncableContainer>();
            syncables.Add("Tracker", syncableComponent);

            syncableComponent.Initialize();
            containerComp.uuid = Guid.NewGuid().ToString();
            containerComp.name = "Tracker";
            containerComp.syncables = syncables;
            containerComp.type = SyncableContainerType.Tracker;
            containerComp.model = model;
            SynchronizationManager.Register(containerComp);
            containerComp.SendUpdate();
        }
    }
}