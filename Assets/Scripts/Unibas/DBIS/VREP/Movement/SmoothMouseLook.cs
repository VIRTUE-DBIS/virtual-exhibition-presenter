using System.Collections.Generic;
using UnityEngine;

namespace Unibas.DBIS.VREP.Movement
{
  /// <summary>
  /// From http://wiki.unity3d.com/index.php/SmoothMouseLook
  /// </summary>
  [AddComponentMenu("Camera-Control/Smooth Mouse Look")]
  public class SmoothMouseLook : MonoBehaviour
  {
    public enum RotationAxes
    {
      MouseXAndY = 0,
      MouseX = 1,
      MouseY = 2
    }

    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    private float _rotationX;
    private float _rotationY;

    private readonly List<float> _rotArrayX = new List<float>();
    private float _rotAverageX;

    private readonly List<float> _rotArrayY = new List<float>();
    private float _rotAverageY;

    public float frameCounter = 20;

    private Quaternion _originalRotation;

    private void Update()
    {
      switch (axes)
      {
        case RotationAxes.MouseXAndY:
        {
          _rotAverageY = 0f;
          _rotAverageX = 0f;

          _rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
          _rotationX += Input.GetAxis("Mouse X") * sensitivityX;

          _rotArrayY.Add(_rotationY);
          _rotArrayX.Add(_rotationX);

          if (_rotArrayY.Count >= frameCounter)
          {
            _rotArrayY.RemoveAt(0);
          }

          if (_rotArrayX.Count >= frameCounter)
          {
            _rotArrayX.RemoveAt(0);
          }

          foreach (var t in _rotArrayY)
          {
            _rotAverageY += t;
          }

          foreach (var t in _rotArrayX)
          {
            _rotAverageX += t;
          }

          _rotAverageY /= _rotArrayY.Count;
          _rotAverageX /= _rotArrayX.Count;

          _rotAverageY = ClampAngle(_rotAverageY, minimumY, maximumY);
          _rotAverageX = ClampAngle(_rotAverageX, minimumX, maximumX);

          var yQuaternion = Quaternion.AngleAxis(_rotAverageY, Vector3.left);
          var xQuaternion = Quaternion.AngleAxis(_rotAverageX, Vector3.up);

          transform.localRotation = _originalRotation * xQuaternion * yQuaternion;
          break;
        }
        case RotationAxes.MouseX:
        {
          _rotAverageX = 0f;

          _rotationX += Input.GetAxis("Mouse X") * sensitivityX;

          _rotArrayX.Add(_rotationX);

          if (_rotArrayX.Count >= frameCounter)
          {
            _rotArrayX.RemoveAt(0);
          }

          foreach (var t in _rotArrayX)
          {
            _rotAverageX += t;
          }

          _rotAverageX /= _rotArrayX.Count;

          _rotAverageX = ClampAngle(_rotAverageX, minimumX, maximumX);

          var xQuaternion = Quaternion.AngleAxis(_rotAverageX, Vector3.up);
          transform.localRotation = _originalRotation * xQuaternion;
          break;
        }
        default:
        {
          _rotAverageY = 0f;

          _rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

          _rotArrayY.Add(_rotationY);

          if (_rotArrayY.Count >= frameCounter)
          {
            _rotArrayY.RemoveAt(0);
          }

          foreach (var t in _rotArrayY)
          {
            _rotAverageY += t;
          }

          _rotAverageY /= _rotArrayY.Count;

          _rotAverageY = ClampAngle(_rotAverageY, minimumY, maximumY);

          var yQuaternion = Quaternion.AngleAxis(_rotAverageY, Vector3.left);
          transform.localRotation = _originalRotation * yQuaternion;
          break;
        }
      }
    }

    private void Start()
    {
      var rb = GetComponent<Rigidbody>();
      if (rb)
        rb.freezeRotation = true;
      _originalRotation = transform.localRotation;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
      angle %= 360;
      if (!(angle >= -360F) || !(angle <= 360F)) return Mathf.Clamp(angle, min, max);

      if (angle < -360F)
      {
        angle += 360F;
      }

      if (angle > 360F)
      {
        angle -= 360F;
      }

      return Mathf.Clamp(angle, min, max);
    }
  }
}