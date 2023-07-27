using UnityEngine;
using Sirenix.OdinInspector;

namespace Toolbox.Singleton
{
    ///<summary>Once throughout all scenes. Important! override Awake</summary>
    /* public class ClassName : SingletonPersistent<ClassName> {
     *      public override void Awake()  {base.Awake(); ... Do Stuff }*/
    public class SingletonPersistent<T> : MonoBehaviour where T : Component
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
        static T _instance; // keeping track of instance in this class

        public static T Instance
        {
            get { return _instance; }
            set { }
        }

        public virtual void Awake()
        {
            if (_instance != null)
                Destroy(this);
            else
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_instance != this)
                _instance = null;
        }
    }
}