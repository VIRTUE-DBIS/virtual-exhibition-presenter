using System;
using System.Collections;
using System.Text;
using DefaultNamespace.VREM.Model;
using UnityEngine;

namespace DefaultNamespace.VREM
{
    public class RESTClient : MonoBehaviour
    {
        public string ServerUrl;
        private string suffix;

        private string response;

        private Action<string> responseProcessor;

        /// <summary>
        /// Requests an exhibition and calls the processor, once the exhibition is loaded.
        /// </summary>
        /// <param name="exhibitionId">The ID of the exhibition</param>
        /// <param name="processor">An Action which processes VREM's response</param>
        public void RequestExhibition(string exhibitionId, Action<string> processor)
        {
            this.suffix = exhibitionId;
            responseProcessor = processor;
            StartCoroutine(DoExhibitionRequest());
        }

        private IEnumerator DoExhibitionRequest()
        {
            Debug.Log("[RC] Requesting...");
            WWW www = new WWW(ServerUrl + "exhibitions/load/" + suffix);
            yield return www;
            if (www.error == null)
            {
                response = www.text;
                if (responseProcessor != null)
                {
                    responseProcessor.Invoke(response);
                }
            }
            else
            {
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