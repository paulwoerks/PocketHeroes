using System.Collections;
using UnityEngine;
using Toolbox.Pooling;

public class DespawnTimer : MonoBehaviour
{
    [SerializeField] float time = 1f;
    public bool IsRunning => Wait != null;
    Coroutine Wait;
    void OnEnable(){
        if (time > 0f)
            Wait = StartCoroutine(WaitForDespawn());
    }

    void OnDisable(){
        if (!IsRunning)
            return;

        StopCoroutine(Wait);
        Wait = null;
    }

    IEnumerator WaitForDespawn(){
        yield return new WaitForSeconds(time);
        Pooler.Despawn(gameObject);
        Wait = null;
    }
}
