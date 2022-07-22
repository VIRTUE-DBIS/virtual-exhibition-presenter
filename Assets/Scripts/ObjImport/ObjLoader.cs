using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace ObjImport
{
  public class ObjLoader : MonoBehaviour
  {
    public void Load(string url)
    {
      StartCoroutine(LoadModel(url));
    }

    private IEnumerator LoadModel(string url)
    {
      Debug.Log("Loading model " + url + ".");
      using var request = UnityWebRequest.Get(url);

      yield return request.SendWebRequest();
      if (!(request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError))
      {
        Debug.Log(request.downloadHandler.text);
        var holderMesh = FastObjImporter.Instance.ImportFile(request.downloadHandler.text);

        gameObject.AddComponent<MeshRenderer>();
        var filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = holderMesh;
      }
      else
      {
        Debug.LogError(request.error);
      }
    }
  }
}