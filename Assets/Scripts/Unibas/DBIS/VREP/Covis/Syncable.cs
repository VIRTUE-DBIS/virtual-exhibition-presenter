using System;
using Unibas.DBIS.VREP.Network;
using UnityEngine;

namespace Unibas.DBIS.VREP.Covis
{
    public class Syncable : MonoBehaviour
    {
        public String uuid;

        private Vector3 lastPosition;
        private Quaternion lastRotation;
        private Rigidbody rb;
        private bool isRbNotNull;
        private bool initialized = false;

        public void Initialize()
        {
            if (!initialized)
            {
                if (!string.IsNullOrEmpty(uuid))
                {
                    Debug.LogError("Uuid already initialized at syncable initialize!");
                }
                uuid = Guid.NewGuid().ToString();
                Debug.Log("Uuid: " + uuid);
                lastPosition = transform.position;
                lastRotation = transform.rotation;
                rb = GetComponent<Rigidbody>();
                isRbNotNull = rb != null;
            }

            initialized = true;
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
                if (usePosition)
                {
                    syncable.Velocity = ToVec3(rb.velocity);
                }
                
                if (useRotation)
                {
                    syncable.AngularVelocity = ToVec3(rb.angularVelocity);
                }
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

        private void UpdateVelocity(Vec3 velocity)
        {
            rb.velocity = FromVec3(velocity);
        }

        private void UpdateAngluarVelocity(Vec3 angularVelocity)
        {
            rb.angularVelocity = FromVec3(angularVelocity);
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

                    if (isRbNotNull)
                    {
                        if (syncable.Velocity != null)
                        {
                            UpdateVelocity(syncable.Velocity);
                        }

                        if (syncable.AngularVelocity != null)
                        {
                            UpdateAngluarVelocity(syncable.AngularVelocity);
                        }
                    }
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