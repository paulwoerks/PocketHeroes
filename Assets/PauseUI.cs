using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    [SerializeField] Image pauseSymbol;
    [SerializeField] GameObject joystick;
    [SerializeField] GameObject pauseMenu;

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            toggle.isOn = !toggle.isOn;
            //SetPause(!pauseMenu.activeSelf);
        }
    }
    public void SetPause(bool isPause){
        pauseSymbol.enabled = !isPause;
        pauseMenu?.SetActive(isPause);
        joystick.SetActive(!isPause);

        if (isPause){
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }
}
