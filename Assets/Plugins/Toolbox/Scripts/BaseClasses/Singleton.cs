using Sirenix.OdinInspector;

/*------------------------------------------------------+
 * Author:  Paul Woerner                                |
 * Email:   code@paulwoerner.com                        |
 +------------------------------------------------------*/

// Info:    Treats 3D objects like buttons
// Ref:     https://www.youtube.com/watch?v=Ova7l0UB26U

namespace Toolbox.Singleton
{
    using UnityEngine;

    ///<summary>Once Per Scene</summary>
    // Example Use: public class ClassName : Singleton<ClassName> { ... Do Stuff } 
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public bool debug;
#if UNITY_EDITOR
        public bool showNotes = false;

        [ShowIf("@showNotes")]
        [Title("Notes", bold: false)]
        [HideLabel]
        [MultiLineProperty(3)]

        [SerializeField] string notes;

        public string Notes => notes;
#endif
        private static T _instance;
        private static readonly object _instanceLock = new object();
        private static bool _quitting = false;

        public static T Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null && !_quitting)
                    {

                        _instance = GameObject.FindObjectOfType<T>();
                        if (_instance == null)
                        {
                            GameObject go = new GameObject(typeof(T).ToString());
                            _instance = go.AddComponent<T>();

                            DontDestroyOnLoad(_instance.gameObject);
                        }
                    }

                    return _instance;
                }
            }
        }

        public virtual void Awake()
        {
            if (_instance == null) _instance = gameObject.GetComponent<T>();
            else if (_instance.GetInstanceID() != GetInstanceID())
            {
                Destroy(gameObject);
                this.Log(string.Format($"Instance of {0} already exists, removing {1}", GetType().FullName, ToString()), debug);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _quitting = true;
        }
    }
}
