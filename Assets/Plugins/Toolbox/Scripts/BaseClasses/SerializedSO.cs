using UnityEngine;
using Sirenix.OdinInspector;

// BY  : Paul Woerner, info@paulwoerner.com
// RES : Unity Official: 'Chop Chop': https://youtu.be/WLDgtRNK2VE

namespace Toolbox
{
    /// <summary>SO with note and debug function that is serialized (can show values of Dictionary for example)</summary>
    public class SerializedSO : SerializedScriptableObject
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
    }
}