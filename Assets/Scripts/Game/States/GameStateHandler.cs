using UnityEngine;
using System.Collections;
using Toolbox.Events;
public class GameStateHandler : MonoBehaviour
{
    [SerializeField] Animator states;
    
    [Header("Listening to ...")]
    [SerializeField] VoidChannelSO OnPlayerDie;
    [SerializeField] VoidChannelSO OnWaveEnd;

    [SerializeField] VoidChannelSO OnSkillsChosen;

    [SerializeField] VoidChannelSO OnCloseMerchant;

    void OnEnable(){
        OnPlayerDie?.Subscribe(SetPlayerDead, this);
        OnWaveEnd?.Subscribe(EndWave, this);
        OnSkillsChosen?.Subscribe(SetSkillsChosen, this);
        OnCloseMerchant?.Subscribe(SetCloseMerchant, this);
    }

    void OnDisable(){
        OnPlayerDie?.Unsubscribe(SetPlayerDead, this);
        OnWaveEnd?.Unsubscribe(EndWave, this);
        OnSkillsChosen?.Unsubscribe(SetSkillsChosen, this);
        OnCloseMerchant?.Unsubscribe(SetCloseMerchant, this);
    }

    void SetPlayerDead(){
        states.SetBool("IsPlayerDead", true);
    }

    void EndWave(){
        Trigger("OnWaveEnded");
        Trigger("OnWaveEnded");
    }

    public void SetSkillsChosen(){
        Trigger("OnSkillsChosen");
    }

    public void SetCloseMerchant(){
        Trigger("OnCloseMerchant");
    }

    void Trigger(string triggerName) => StartCoroutine(SetTrigger(triggerName));

    IEnumerator SetTrigger(string triggerName){
        states.SetTrigger(triggerName);
        yield return null;
        states.ResetTrigger(triggerName);
    }
}
