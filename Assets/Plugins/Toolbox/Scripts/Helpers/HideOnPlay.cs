using UnityEngine;

namespace Toolbox
{
    public class HideOnPlay : MonoBehaviour
    {
        void Awake() => gameObject.SetActive(false);
    }
}
