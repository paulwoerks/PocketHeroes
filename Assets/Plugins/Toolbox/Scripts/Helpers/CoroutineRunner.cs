using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Toolbox.Coroutines
{
    ///<summary>Runs Coroutines for Scripts that can't run them natively</summary>
    public class CoroutineRunner : MonoBehaviour
    {
        [SerializeField] bool debug = false;

        [SerializeField] List<Coroutine> activeCoroutines = new();

        #region Singleton
        private static readonly object _instanceLock = new object();
        private static bool _quitting = false;

        private static CoroutineRunner _instance;
        public static CoroutineRunner Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null && !_quitting)
                    {
                        _instance = GameObject.FindObjectOfType<CoroutineRunner>();

                        if (_instance == null)
                        {
                            _instance = new GameObject("CoroutineRunner").AddComponent<CoroutineRunner>();
                            DontDestroyOnLoad(_instance.gameObject);
                        }
                    }

                    return _instance;
                }
            }
        }

        void Awake()
        {
            if (_instance == null) _instance = gameObject.GetComponent<CoroutineRunner>();
            else if (_instance.GetInstanceID() != GetInstanceID())
            {
                Destroy(gameObject);
                this.Log(string.Format($"Instance of {0} already exists, removing {1}", GetType().FullName, ToString(), debug));
            }
        }

        void OnApplicationQuit()
        {
            _quitting = true;
        }
        #endregion
        void OnDisable() => StopAll();

        public static Coroutine Start(IEnumerator coroutine)
        {
            Coroutine routine = Instance.StartCoroutine(coroutine);
            Instance.activeCoroutines.Add(routine);
            return routine;
        }

        public static void Stop(Coroutine coroutine)
        {
            Instance.StopCoroutine(coroutine);
            Instance.activeCoroutines.Remove(coroutine);
        }

        void StopAll()
        {
            foreach (Coroutine coroutine in activeCoroutines)
                StopCoroutine(coroutine);

            this.Log($"{activeCoroutines.Count} stopped", debug);
            activeCoroutines = new();
        }
    }
}