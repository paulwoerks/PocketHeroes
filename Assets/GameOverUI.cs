using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Toolbox.Events;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] GameObject ui;

    [Header("Listening to ...")]
    [SerializeField] VoidChannelSO OnGameOver;

    void OnEnable() => OnGameOver.Subscribe(SetActive, this);
    void OnDisable() => OnGameOver.Unsubscribe(SetActive, this);
    void SetActive(){
        StartCoroutine(ScreenDelay());
    }

    IEnumerator ScreenDelay(){
        yield return new WaitForSeconds(1);
        ui.SetActive(true);
    }

    public void Restart(){
        SceneManager.LoadScene(0);
    }
}
