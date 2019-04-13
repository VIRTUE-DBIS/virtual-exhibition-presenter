using System.Collections;
using System.Linq.Expressions;
using DefaultNamespace;
using UnityEngine;
using World;

namespace Unibas.DBIS.VREP.Puzzle {
  public class PuzzleManager : MonoBehaviour {
    private static PuzzleManager _instance;

    private void Awake() {
      _instance = this;
    }

    public static PuzzleManager GetInstance() {
      return _instance;
    }


    private GameObject[] _cubes;

    public bool IsPuzzleActive() {
      return _cubes != null;
    }

    public void SetPuzzle(GameObject[] cubes) {
      _cubes = cubes;
    }

    public void RemovePuzzle() {
      if (_cubes != null) {
        foreach (var cube in _cubes) {
          DestroyImmediate(cube);
        }

        _cubes = null;
        
      }
    }

    public IEnumerator PreparePuzzleForDisplayal(Displayal displayal) {
      yield return new WaitForSeconds(1);

      Transform displayalParent = displayal.transform.parent;
      var offset = displayal.GetExhibit().size.y / 2;
      var model = new SteamVRPuzzleButton.ButtonModel(0.2f, .02f, 2f, TexturingUtility.LoadMaterialByName("none"),
        TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic"),
        hasPedestal: false);
      var postiion =
        displayal.GetExhibit().position - new Vector3(0, (offset + 0.5f), 0.2f); // why y and z other than expected?

      var button = SteamVRPuzzleButton.Create(displayalParent.gameObject, postiion,
        displayal, model, "Puzzle");
      button.name = "Puzzle Button (" + displayal.id + ")";
    }
  }
}