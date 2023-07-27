using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using SimpleJSON;
using Toolbox.Coroutines;

namespace Toolbox.API
{
    /// <summary>Reads simple Server Requests and gives the results back to a function</summary>
    public static class APIReader
    {
        /// <summary>Read Data from a server, returning JSONNode with root</summary>
        /// <param name="API_URL">Put in the https:// URL u wanna call</param>
        /// <param name="GetResult">Provide a method that the root Node is returned to</param>
        /// /// <param name="HandleError">Provide a Method to Hanlde errors - Otherwise just displayed by API Reader</param>
        /// <param name="timeout">The optional timeout in seconds.</param>
        public static void Read(string API_URL, Action<JSONNode> GetResult, Action<string, string> HandleError = null, float timeout = 10f)
        {
            CoroutineRunner.Start(ReadFromServer(API_URL, GetResult, HandleError, timeout));
        }

        /// <summary>Fetches Data from a Server, handled in a Coroutine inside the Coroutine Runner</summary>
        static IEnumerator ReadFromServer(string API_URL, Action<JSONNode> GetResult, Action<string, string> HandleError = null, float timeout = 10f)
        {
            UnityWebRequest request = UnityWebRequest.Get(API_URL);
            request.timeout = Mathf.FloorToInt(timeout);

            yield return request.SendWebRequest();

            if (IsRequestSuccessful(request, HandleError))
                GetResult(JSON.Parse(request.downloadHandler.text));
        }


        /// <summary>Checks Web Requests for Errors and returns if successful</summary>
        /// <returns>True or Lists Error</returns>
        static bool IsRequestSuccessful(UnityWebRequest request, Action<string, string> HandleError)
        {
            if (request.result == UnityWebRequest.Result.Success)
                return true;

            if (HandleError != null)
                HandleError(request.result.ToString(), request.error.ToString());
            else
                Debug.LogError($"{request.result.ToString()}: {request.error.ToString()}");
            return false;
        }
    }
}