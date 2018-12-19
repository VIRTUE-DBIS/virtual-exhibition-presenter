using System;
using System.Collections;
using System.Text;
using DefaultNamespace.VREM.Model;
using UnityEngine;

namespace DefaultNamespace.VREM {
  public class RESTClient : MonoBehaviour{

    public string ServerUrl;
    private string suffix;
    
    private string response;

    private Action<string> responseProcessor;

    public void RequestExhibition(string suffix, Action<string> processor) {
      this.suffix = suffix;
      responseProcessor = processor;
      StartCoroutine(Request());
    }

    private IEnumerator Request() {
      Debug.Log("[RC] Requesting...");
      WWW www = new WWW(ServerUrl+suffix);
      yield return www;
      if (www.error == null) {
        response = www.text;
        if (responseProcessor != null) {
          responseProcessor.Invoke(response);
        }
      } else {
        // Error, handle it!
      }
    }
    
    
    
    /**
         * Generates a WWW object with given params
         * 
         * @param url - A string which represents the url
         * @param json - The json data to send, as a string
         * 
         */
    public static WWW GenerateJSONPostRequest(string url, string json)
    {
      Hashtable headers = new Hashtable();
      headers.Add("Content-Type", "application/json");
      byte[] postData = Encoding.ASCII.GetBytes(json.ToCharArray());
      return new WWW(url,
        postData);
    }
    
  }
}