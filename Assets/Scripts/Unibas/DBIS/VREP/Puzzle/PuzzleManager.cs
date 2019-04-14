using System;
using System.Collections;
using System.Linq.Expressions;
using DefaultNamespace;
using DefaultNamespace.VREM.Model;
using UnityEngine;
using World;

namespace Unibas.DBIS.VREP.Puzzle
{
    public class PuzzleManager : MonoBehaviour
    {
        private static PuzzleManager _instance;

        private void Awake()
        {
            _instance = this;
        }

        public static PuzzleManager GetInstance()
        {
            return _instance;
        }


        private GameObject[] _cubes;
        public Vector2Int nbCubes;

        public bool IsPuzzleActive()
        {
            return _cubes != null;
        }

        private void SetPuzzle(GameObject[] cubes)
        {
            _cubes = cubes;
        }

        [SerializeField]
        private bool[,] _positionCheckMatrix;
        public bool[,] GetPositionCheckMatrix
        {
            get
            {
                return _positionCheckMatrix; 
            }
            set
            {
                this._positionCheckMatrix = value;
            }
        }

        public Vector2Int GetPosForId(int id)
        {
            if (nbCubes != null)
            {
                return new Vector2Int(id % nbCubes.x, id / nbCubes.x);
            }
            else
            {
                return new Vector2Int(-1, -1);
            }
        }

        public int GetIdForPos(int x, int y)
        {
            if (nbCubes != null)
            {
                return x * nbCubes.x + y;
            }
            else
            {
                return -1;
            }
        }

        public void SetPuzzle(Displayal displayal)
        {
            if (!GetInstance().IsPuzzleActive())
            {
                var otherDisp = GetRandomOtherDisplayal(displayal);
                Debug.Log("Creating puzzle for "+displayal.id +" with other: "+otherDisp.id);
                var tex = displayal.GetDisplayalRenderer().material.mainTexture;
                nbCubes = PuzzleCubeFactory.getNumberOfCubes(tex.width, tex.height);
                var other = otherDisp.GetDisplayalRenderer().material.mainTexture;
                var cubes = PuzzleCubeFactory.CreatePuzzle(tex, 0.5f, displayal.RoomPosition+new Vector3(2.5f,0,2.5f), other); // TODO Magic size
                GetInstance().SetPuzzle(cubes);
                Debug.Log("Cubes there?!");
                _positionCheckMatrix = new bool[nbCubes.y, nbCubes.x];
                StartCoroutine(RestorePositionChecks());
            }
        }

        private IEnumerator RestorePositionChecks()
        {
            yield return new WaitForSeconds(4);
            foreach (var go in _cubes)
            {
                go.GetComponent<PuzzleCube>().RestoreInitialNeighborChecks();
            }
        }

        private Displayal GetRandomOtherDisplayal(Displayal displayal)
        {
            var exhibits = VREPController.Instance.GetExhibitionManager().GetExhibition().GetExhibits();
            System.Random r = new System.Random();

            Exhibit e;
            do
            {
                e = exhibits[r.Next(0, exhibits.Length - 1)];
            } while (e.id == displayal.id);

            return VREPController.Instance.GetDisplayalForId(e.id);
        }

        public void RemovePuzzle()
        {
            if (_cubes != null)
            {
                foreach (var cube in _cubes)
                {
                    DestroyImmediate(cube);
                }

                _cubes = null;
                nbCubes = new Vector2Int(-1,-1);
            }
        }

        public void SetPositionCheck(int x, int y, bool check)
        {
            if (IsPuzzleActive())
            {
                Debug.Log("Setting ("+x+","+y+") to "+check);
                _positionCheckMatrix[y, x] = check;
            }
        }

        private void Update()
        {
            if (IsPuzzleActive())
            {
                int correct = 0;
                foreach (bool check in _positionCheckMatrix)
                {
                    if (check)
                    {
                        correct++;
                    }
                }

                if (correct == _cubes.Length)
                {
                    PuzzleComplete();
                }
            }
        }

        public void PuzzleComplete()
        {
            Debug.Log("Puzzle Complete");
            RemovePuzzle();
            
        }

        public IEnumerator PreparePuzzleForDisplayal(Displayal displayal)
        {
            yield return new WaitForSeconds(1);

            Transform displayalParent = displayal.transform.parent;
            var offset = displayal.GetExhibit().size.x / 2;
            var model = new SteamVRPuzzleButton.ButtonModel(0.2f, .02f, 2f, TexturingUtility.LoadMaterialByName("none"),
                TexturingUtility.LoadMaterialByName("NMetal"), TexturingUtility.LoadMaterialByName("NPlastic"),
                hasPedestal: false);
            var postiion =
                displayal.GetExhibit().position +
                new Vector3(offset + 0.5f, 0, -0.1f); // why y and z other than expected?

            var button = SteamVRPuzzleButton.Create(displayalParent.gameObject, postiion,
                displayal, model, "Puzzle");
            /*var button = SteamVRPuzzleButton.Create(displayalParent.gameObject, postiion,
              displayal, model);*/
            button.name = "Puzzle Button (" + displayal.id + ")";
        }
    }
}