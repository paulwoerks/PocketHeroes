using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] TextMeshPro textMesh;
    [SerializeField] DespawnTimer despawnTimer;

    float height;

    Transform target;

    public void SetText(int damage, Transform target = null, float height = 1.5f) => SetText(damage.ToString(), target, height);
    public void SetText(string text, Transform target = null, float height = 1.5f){
        this.target = target;
        this.height = height;
        textMesh.SetText(text);
    }

    void Update(){
        if (target != null)
            transform.position = target.position + Vector3.up * height;
    }
}
