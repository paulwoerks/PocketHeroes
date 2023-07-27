using UnityEngine;

// Author: Paul Woerner
// 1. DrowsyFoxDev: 'You are using Debug.Log wrong, do THIS': https://youtu.be/lRqR4YF8iQs
// 2. samyam: 'Introduction to the Console and Types of Logging in Unity': https://youtu.be/YqhMhSLbeuw

namespace Toolbox
{
    ///<summary>Better alternative to the direct approach of Unity Logging.</summary>
    public static class LogReader
    {
        ///<summary>Equivalent to Debug.Log();</summary>
        public static void Log(this Object myObj, object msg, bool debug = true)
        {
            if (!debug)
                return;

            Debug.Log($"[{myObj.name}]: {msg}", myObj);
        }

        ///<summary>Equivalent to Debug.LogWarning();</summary>
        public static void LogWarning(this Object myObj, object msg, bool debug = false)
        {
            if (!debug)
                return;

            string obj = $"[{myObj.name}]".ColorizeString("red");
            Debug.LogError($"{obj}: {msg}", myObj);
        }

        ///<summary>Equivalent to Debug.LogError();</summary>
        public static void LogError(this Object myObj, object msg)
        {
            string obj = $"[{myObj.name}]".ColorizeString("red");
            Debug.LogError($"{obj}: {msg}", myObj);
        }

        ///<summary>Equivalent to Debug.LogException</summary>
        public static void LogException(this Object myObj, System.Exception exception, object message = null)
        {
            message = message == null ? "" : ": " + message;
            string obj = $"[{myObj.name}]".ColorizeString("red");
            Debug.LogError($"{obj}: {exception}{message}", myObj);
        }

        ///<summary>Prints if true</summary>
        // this.LogAssert(3 - 1 = 5, "Optional Message");
        public static void LogAssert(this Object myObj, bool assertation, bool debug = false, object message = null)
        {
            if (!debug || assertation)
                return;

            message = message == null ? message = "!" : message = $": {message}";

            string obj = $"[{myObj.name}]".ColorizeString("magenta");
            Debug.Log($"{obj}: Assert false{message}", myObj);
        }

        static string ColorizeString(this string str, string color) => $"<color={color}>{str}</color>";
    }
}