using System;
using System.Collections;
using System.Collections.Generic;
using Unibas.DBIS.VREP.VREM.Model;
using UnityEngine;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;

namespace Unibas.DBIS.VREP.VREM
{
  /// <summary>
  /// VREM client handling requests to the VREM API to load exhibitions/exhibits. 
  /// </summary>
  public class VremClient : MonoBehaviour
  {
    private const string LoadExhibitionAction = "exhibitions/load/";
    private const string GenerateExhibitionAction = "generate/";
    private const string ListExhibitionsAction = "exhibitions/list";
    public string serverUrl;

    private bool _error;
    private string _response;

    private Action<string> _responseProcessor;
    private string _suffix;

    /// <summary>
    /// Requests an exhibition and calls the processor, once the exhibition is loaded.
    /// </summary>
    /// <param name="exhibitionId">The ID of the exhibition.</param>
    /// <param name="processor">An Action which processes VREM's response (e.g., VrepController.ParseExhibition())</param>
    public void RequestExhibition(string exhibitionId, GenerationConfig genConfig, Action<string> processor)
    {
      // TODO Refactor Action to a proper interface.
      _suffix = exhibitionId;
      _responseProcessor = processor;
      StartCoroutine(DoExhibitionRequest(genConfig));
    }

    public void RequestRoom(GenerationConfig conf, Action<string> processor)
    {
      _responseProcessor = processor;
      StartCoroutine(DoRoomRequestFromEx(conf));
    }

    /// <summary>
    /// Executes an exhibition request based on the parameters that have been set previously.
    /// Invoked via Coroutine.
    /// </summary>
    /// <returns>The result yielded from the request.</returns>
    private IEnumerator DoExhibitionRequest(GenerationConfig genConfig)
    {
      SanitizeServerUrl();

      Debug.Log("[VREMClient] Requesting " + serverUrl + GenerateExhibitionAction + _suffix + ".");

      // using var request = UnityWebRequest.Get(serverUrl + LoadExhibitionAction + _suffix);
      // yield return request.SendWebRequest(); 
      var request = new UnityWebRequest(serverUrl + GenerateExhibitionAction, "POST");

      var json = JsonConvert.SerializeObject(genConfig);

      request.uploadHandler = new UploadHandlerRaw(new System.Text.UTF8Encoding().GetBytes(json));
      request.SetRequestHeader("Content-Type", "application/json");
      request.downloadHandler = new DownloadHandlerBuffer();

      yield return request.SendWebRequest();

      if (!(request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError))
      {
        _response = request.downloadHandler.text;
        _responseProcessor?.Invoke(_response);
      }
      else
      {
        Debug.LogError(request.error);
        // TODO Error, handle it!
        _error = true;
        _responseProcessor.Invoke(null);
      }
    }

    private IEnumerator DoRoomRequestFromEx(GenerationConfig genConfig)
    {
      var request = new UnityWebRequest("http://localhost:4545/generate", "POST");

      var json = JsonConvert.SerializeObject(genConfig);

      request.uploadHandler = new UploadHandlerRaw(new System.Text.UTF8Encoding().GetBytes(json));
      request.SetRequestHeader("Content-Type", "application/json");
      request.downloadHandler = new DownloadHandlerBuffer();

      yield return request.SendWebRequest();

      if (!(request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError))
      {
        _response = request.downloadHandler.text;
        _responseProcessor?.Invoke(_response);
      }
      else
      {
        Debug.LogError(request.error);
        // TODO Error, handle it!
      }
    }

    /// <summary>
    /// Executes a request to list all stored exhibitions (IDs & names, as in ExhibitionSummary objects).
    /// </summary>
    /// <returns>The result yielded from the request.</returns>
    private IEnumerator DoListExhibitionRequest()
    {
      // TODO Implement this.
      SanitizeServerUrl();

      Debug.Log("[VREMClient] Requesting exhibition list.");

      using var request = UnityWebRequest.Get(serverUrl + ListExhibitionsAction);
      yield return request.SendWebRequest();

      if (!(request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError))
      {
        _response = request.downloadHandler.text;
        // TODO Parse list of IDs and names of the ExhibitionSummary objects.
      }
      else
      {
        Debug.LogError(request.error);
        // TODO Proper error handling.
        _error = true;
        _responseProcessor.Invoke(null);
      }
    }

    /// <summary>
    /// Fixes the server URL by adding the http:// prefix and/or trailing /.
    /// </summary>
    private void SanitizeServerUrl()
    {
      if (!serverUrl.StartsWith("http://"))
      {
        serverUrl = "http://" + serverUrl;
      }

      if (!serverUrl.EndsWith("/"))
      {
        serverUrl += "/";
      }
    }

    /// <summary>
    /// Checks whether this VremClient instance has encountered an error.
    /// </summary>
    /// <returns>True upon an encountered error, false otherwise.</returns>
    public bool HasError()
    {
      return _error;
    }
  }
}