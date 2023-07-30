using UnityEngine;
using Toolbox.Events;

public class EventBasedUI : MonoBehaviour
{
    [SerializeField] GameObject UI;

    [Header("Listening to ...")]
    [SerializeField] VoidChannelSO OnShow;
    [SerializeField] VoidChannelSO OnHide;

    void OnEnable(){
        OnShow?.Subscribe(Show, this);
        OnHide?.Subscribe(Hide, this);
    }
    void OnDisable(){
        OnShow?.Unsubscribe(Show, this);
        OnHide?.Unsubscribe(Hide, this);
    } 

    public virtual void Show(){
        UI.SetActive(true);
    }

    public virtual void Hide(){
        UI.SetActive(false);
    }
}
