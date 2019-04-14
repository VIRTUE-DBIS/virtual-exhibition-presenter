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
        private Displayal Displayal;

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

        public int GetIdForPos(Vector2Int pos)
        {
            return GetIdForPos(pos.x, pos.y);
        }

        public int GetIdForPos(int x, int y)
        {
            if (nbCubes != null)
            {
                return y * nbCubes.x + x;
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
                this.Displayal = displayal;
                StartCoroutine(RestorePositionChecks());
            }
        }

        private IEnumerator RestorePositionChecks()
        {
            yield return new WaitForSeconds(4);
            foreach (var go in _cubes)
            {
                go.GetComponent<PuzzleCube>().RestoreInitialNeighborChecks();
                go.GetComponent<PuzzleCube>().DoTriggerChecks = true;
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
                    DestroyImmediate(cube.transform.parent.gameObject);
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

        private bool _finish = false;
        
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

                if (correct == _cubes.Length && CheckPuzzleDistances())
                {
                    PuzzleComplete();
                }
            }
        }

        private GameObject fireworks;
        
        public void PuzzleComplete()
        {
            if (_finish)
            {
                return;
            }
            Debug.Log("Puzzle Complete");
            var prefab = VREPController.Instance.GetBuildingManager().FireworksPrefab;
            fireworks = Instantiate(prefab);
            fireworks.transform.position = Displayal.RoomPosition;
            StartCoroutine(StopFireWorks());
            _finish = true;
        }

        public IEnumerator StopFireWorks()
        {
            yield return new WaitForSecondsRealtime(30);
            Destroy(fireworks);
            RemovePuzzle();
        }

        public bool WithinPuzzle(Vector2Int pos)
        {
            return (0 <= pos.x && pos.x < nbCubes.x) && (0 <= pos.y && pos.y < nbCubes.y);
        }

        public bool CheckPuzzleDistances()
        {
            var counter = 0;
            var cube = _cubes[0].GetComponent<PuzzleCube>();
            for (int i = 0; i < _cubes.Length; i++)
            {
                var c1 = _cubes[i].GetComponent<PuzzleCube>();
                Debug.Log("C("+c1.Id+") dist: "+c1.CheckDistances());
                if (c1.CheckDistances())
                {
                    counter++;
                }
            }
            Debug.Log("Puzzle Distances: "+counter);
            return counter == _cubes.Length;
        }

        public GameObject GetForId(int id)
        {
            return _cubes[id];
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