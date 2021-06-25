using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Unibas.DBIS.VREP.VREM
{
  public class VREMClient : MonoBehaviour
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
    /// <param name="exhibitionId">The ID of the exhibition</param>
    /// <param name="processor">An Action which processes VREM's response. If null is passed to that action, an error occurred</param>
    public void RequestExhibition(string exhibitionId, Action<string> processor)
    {
      // TODO Refactor Action to a proper interface
      _suffix = exhibitionId;
      _responseProcessor = processor;
      StartCoroutine(DoExhibitionRequest());
    }

    private IEnumerator DoExhibitionRequest()
    {
      SanitizeServerUrl();

      Debug.Log("[VREMClient] Requesting... " + serverUrl + LoadExhibitionAction + _suffix);
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

    private IEnumerator DoListExhibitionRequest()
    {
      SanitizeServerUrl();

      Debug.Log("[VREMClient] Requesting exhibition list ");
      using var request = UnityWebRequest.Get(serverUrl + ListExhibitionsAction);
      yield return request.SendWebRequest();
      if (!(request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError))
      {
        _response = request.downloadHandler.text;
        // TODO Parse list of IDs and further loading of the exhibitions.
        // Will induce follow-up DoExhibitonRequests
      }
      else
      {
        Debug.LogError(request.error);
        // TODO Handle error properly
        _error = true;
        _responseProcessor.Invoke(null);
      }
    }

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

    public bool HasError()
    {
      return _error;
    }

    /// <summary>
    /// Generates a WWW object with given params
    /// </summary>
    /// <param name="url">A string which represents the url</param>
    /// <param name="json">The json data to send, as a string</param>
    /// <returns></returns>
    public static WWW GenerateJSONPostRequest(string url, string json)
    {
      var headers = new Hashtable {{"Content-Type", "application/json"}};
      var postData = Encoding.ASCII.GetBytes(json.ToCharArray());
      return new WWW(url, postData);
    }
  }
}