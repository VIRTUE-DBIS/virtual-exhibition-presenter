using System.Threading.Tasks;
using Unibas.DBIS.VREP.Core;
using Unibas.DBIS.VREP.VREM.Model;
using UnityEngine;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;

namespace Unibas.DBIS.VREP.VREM
{
  /// <summary>
  /// VREM client handling requests to the VREM API to load exhibitions/exhibits. 
  /// </summary>
  public class VremClient
  {
    public static VremClient Instance = new VremClient();

    public const string generateApi = "generate/";

    private VremClient()
    {
    }

    public string getHostAddress()
    {
      return VrepController.Instance.settings.VremAddress;
    }

    public async Task<DownloadHandler> JsonPostRequest(string url, object payloadObj)
    {
      var payloadJson = JsonConvert.SerializeObject(payloadObj);

      var request = new UnityWebRequest(url, "POST");
      request.SetRequestHeader("Content-Type", "application/json");
      request.uploadHandler = new UploadHandlerRaw(new System.Text.UTF8Encoding().GetBytes(payloadJson));
      request.downloadHandler = new DownloadHandlerBuffer();

      var op = request.SendWebRequest();

      while (!op.isDone)
      {
        await Task.Yield();
      }

      return request.downloadHandler;
    }

    public async Task<DownloadHandler> GetRequest(string url)
    {
      var request = new UnityWebRequest(url, "GET");
      request.downloadHandler = new DownloadHandlerBuffer();

      var op = request.SendWebRequest();

      while (!op.isDone)
      {
        await Task.Yield();
      }

      return request.downloadHandler;
    }

    public async Task<T> Generate<T>(GenerationConfig genConfig)
    {
      var handler = await JsonPostRequest(getHostAddress() + generateApi, genConfig);

      return JsonConvert.DeserializeObject<T>(handler.text);
    }
  }
}