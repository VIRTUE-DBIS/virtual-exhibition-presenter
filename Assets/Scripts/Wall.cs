using System;
using DefaultNamespace;
using UnityEngine;

public class Wall : MonoBehaviour {
  
  public WallOrientation Orientation;

  public float RoomRadius = 5;
  
/// <summary>
///   Calculates the position relative to the wall. Origin is lower left corner
/// </summary>
/// <param name="floorCenter"></param>
/// <param name="pos">x value is to the right, y value is up</param>
/// <returns></returns>
/// <exception cref="ArgumentOutOfRangeException"></exception>
	public Vector3 CalculatePosition(Vector3 floorCenter, Vector2 pos){
  float epsilon = 0.1f; // Required 0.1f with Displayal
    
    switch (Orientation) {
      case WallOrientation.NORTH:
        return new Vector3(
          floorCenter.x + (RoomRadius - epsilon),
          floorCenter.y + pos.y,
          (floorCenter.z + RoomRadius) - pos.x);
      case WallOrientation.EAST:
        return new Vector3(
          (floorCenter.x + RoomRadius) - pos.x,
          floorCenter.y + pos.y,
          floorCenter.z - (RoomRadius - epsilon));
      case WallOrientation.SOUTH:
        return new Vector3(
          floorCenter.x - (RoomRadius - epsilon),
          floorCenter.y + pos.y,
          (floorCenter.z - RoomRadius) + pos.x);
      case WallOrientation.WEST:
        return new Vector3(
          (floorCenter.x - RoomRadius) + pos.x,
          floorCenter.y + pos.y,
          floorCenter.z + (RoomRadius - epsilon));
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  /// <summary>
  /// 
  /// </summary>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  public Vector3 CalculateRotation() {
    switch (Orientation) {
      case WallOrientation.NORTH:
        return new Vector3(90, 270, 0);
      case WallOrientation.EAST:
        return new Vector3(90, 0, 0);
      case WallOrientation.SOUTH:
        return new Vector3(90, 90, 0);
      case WallOrientation.WEST:
        return new Vector3(90, 180, 0);
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  // Use this for initialization
  private void Start()
  {    
  }
  
  
  public void LoadTexture(string materialName)
  {
    var material = TexturingUtility.getMaterial(materialName);
			
    Renderer rend = GetComponent<Renderer>();
    if (rend != null && material != null)
    {
      rend.material = material;
    }
  }

  // Update is called once per frame
  private void Update() { }
}