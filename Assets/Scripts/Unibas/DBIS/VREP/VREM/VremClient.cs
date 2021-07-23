using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Unibas.DBIS.VREP.VREM
{
  /// <summary>
  /// VREM client handling requests to the VREM API to load exhibitions/exhibits. 
  /// </summary>
  public class VremClient : MonoBehaviour
  {
    public string serverUrl;
    private string _suffix;
    private string _response;

    private const string LoadExhibitionAction = "exhibitions/load/";
    private const string ListExhibitionsAction = "exhibitions/list";

    private Action<string> _responseProcessor;

    /// <summary>
    /// Requests an exhibition and calls the processor, once the exhibition is loaded.
    /// </summary>
    /// <param name="exhibitionId">The ID of the exhibition.</param>
    /// <param name="processor">An Action which processes VREM's response (e.g., VrepController.ParseExhibition())</param>
    public void RequestExhibition(string exhibitionId, Action<string> processor)
    {
      // TODO Refactor Action to a proper interface.
      _suffix = exhibitionId;
      _responseProcessor = processor;
      StartCoroutine(DoExhibitionRequest());
    }

    /// <summary>
    /// Executes an exhibition request based on the parameters that have been set previously.
    /// Invoked via Coroutine.
    /// </summary>
    /// <returns>The result yielded from the request.</returns>
    private IEnumerator DoExhibitionRequest()
    {
      SanitizeServerUrl();

      Debug.Log("[VREMClient] Requesting " + serverUrl + LoadExhibitionAction + _suffix + ".");

      using var request = UnityWebRequest.Get(serverUrl + LoadExhibitionAction + _suffix);
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

    private bool _error;

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