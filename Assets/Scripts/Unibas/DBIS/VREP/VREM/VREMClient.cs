using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace DefaultNamespace.VREM
{
    public class VREMClient : MonoBehaviour
    {
        public string ServerUrl;
        private string suffix;

        private string response;

        private const string LOAD_EXHIBITION_ACTION = "exhibitions/load/";
        private const string LIST_EXHIBITIONS_ACTION = "exhibitions/list";

        private Action<string> responseProcessor;

        /// <summary>
        /// Requests an exhibition and calls the processor, once the exhibition is loaded.
        /// </summary>
        /// <param name="exhibitionId">The ID of the exhibition</param>
        /// <param name="processor">An Action which processes VREM's response. If null is passed to that action, an error occurred</param>
        public void RequestExhibition(string exhibitionId, Action<string> processor)
        {
            // TODO Refactor Action to a proper interface
            this.suffix = exhibitionId;
            responseProcessor = processor;
            StartCoroutine(DoExhibitionRequest());
        }

        private IEnumerator DoExhibitionRequest()
        {
            SanitizeServerUrl();
            
            Debug.Log("[VREMClient] Requesting... "+ServerUrl+LOAD_EXHIBITION_ACTION+suffix);
            using (var request = UnityWebRequest.Get(ServerUrl + LOAD_EXHIBITION_ACTION + suffix))
            {
                yield return request.SendWebRequest();
                if (!(request.isNetworkError || request.isHttpError))
                {
                    response = request.downloadHandler.text;
                    if (responseProcessor != null)
                    {
                        responseProcessor.Invoke(response);
                    }
                }
                else
                {
                    Debug.LogError(request.error);
                    // TODO Error, handle it!
                    error = true;
                    responseProcessor.Invoke(null);
                }
            }
        }

        private IEnumerator DoListExhibitionRequest()
        {
            SanitizeServerUrl();
            
            Debug.Log("[VREMClient] Requesting exhibition list ");
            using (var request = UnityWebRequest.Get(ServerUrl+LIST_EXHIBITIONS_ACTION))
            {
                yield return request.SendWebRequest();
                if (!(request.isNetworkError || request.isHttpError))
                {
                    response = request.downloadHandler.text;
                    // TODO Parse list of IDs and further loading of the exhibitions.
                    // Will induce follow-up DoExhibitonRequests
                }
                else
                {
                    Debug.LogError(request.error);
                    // TODO Handle error properly
                    error = true;
                    responseProcessor.Invoke(null);
                }
            }
        }

        private void SanitizeServerUrl()
        {
            if (!ServerUrl.StartsWith("http://"))
            {
                ServerUrl = "http://" + ServerUrl;
            }

            if (!ServerUrl.EndsWith("/"))
            {
                ServerUrl = ServerUrl + "/";
            }
        }

        private bool error = false;

        public bool HasError()
        {
            return error;
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