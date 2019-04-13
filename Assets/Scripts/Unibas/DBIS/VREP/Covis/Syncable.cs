using System;
using Unibas.DBIS.VREP.Network;
using UnityEngine;

namespace Unibas.DBIS.VREP.Covis
{
    public class Syncable : MonoBehaviour
    {
        public String uuid;
        public bool original;

        private Vector3 lastPosition;
        private Quaternion lastRotation;
        private Rigidbody rb;
        private bool isRbNotNull;

        private void Start()
        {
            lastPosition = transform.position;
            lastRotation = transform.rotation;
            rb = GetComponent<Rigidbody>();
            isRbNotNull = rb != null;
        }

        private Vec3 ToVec3(Vector3 vector)
        {
            Vec3 vec = new Vec3();
            vec.X = vector.x;
            vec.Y = vector.y;
            vec.Z = vector.z;
            return vec;
        }

        private Vec4 ToVec4(Quaternion quaternion)
        {
            Vec4 vec = new Vec4();
            vec.X = quaternion.x;
            vec.Y = quaternion.y;
            vec.Z = quaternion.z;
            vec.W = quaternion.w;
            return vec;
        }

        private Vector3 FromVec3(Vec3 vec)
        {
            return new Vector3((float) vec.X, (float) vec.Y, (float) vec.Z);
        }

        private Quaternion FromVec4(Vec4 vec)
        {
            return new Quaternion((float) vec.X, (float) vec.Y, (float) vec.Z, (float) vec.W);
        }

        public global::Syncable toProtoSyncable(bool usePosition = true, bool useRotation = true)
        {
            global::Syncable syncable = new global::Syncable();
            syncable.Uuid = uuid;
            if (usePosition)
            {
                syncable.Position = ToVec3(transform.position);
            }

            if (useRotation)
            {
                syncable.Rotation = ToVec4(transform.rotation);
            }

            if (isRbNotNull)
            {
                // TODO: Add rb data to message
            }

            return syncable;
        }

        private void UpdatePosition(Vec3 position)
        {
            transform.position = FromVec3(position);
        }

        private void UpdateRotation(Vec4 rotation)
        {
            transform.rotation = FromVec4(rotation);
        }

        void Update()
        {
            var queue = SynchronizationManager.Instance.SyncableUpdateQueue[uuid];
            if (!queue.IsEmpty)
            {
                UpdateMessage message;
                while (!queue.IsEmpty && queue.TryDequeue(out message))
                {
                    var syncable = message.Syncable;
                    if (syncable.Position != null)
                    {
                        UpdatePosition(syncable.Position);
                    }

                    if (syncable.Rotation != null)
                    {
                        UpdateRotation(syncable.Rotation);
                    }

                    // TODO: Update rigidbody
                }
            }
            else
            {
                var positionChanged = transform.position != lastPosition;
                var rotationChanged = transform.rotation != lastRotation;
                if (positionChanged || rotationChanged)
                {
                    UpdateMessage message = new UpdateMessage();

                    message.Syncable = toProtoSyncable(positionChanged, rotationChanged);
                    // TODO: Timestamp

                    CovisClientImpl.Instance.Update(message);
                }
            }

            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }
    }
}