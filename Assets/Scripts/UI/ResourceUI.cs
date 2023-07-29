using UnityEngine;
using TMPro;
using Toolbox.Events;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField] IntChannelSO OnUpdate;

    void OnEnable(){
        OnUpdate?.Subscribe(UpdateUI, this);
    }

    void OnDisable(){
        OnUpdate?.Unsubscribe(UpdateUI, this);
    }
    void UpdateUI(int value){
        textMesh.SetText(value.ToString());
    }
}
