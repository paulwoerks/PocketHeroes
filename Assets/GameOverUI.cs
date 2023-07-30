using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameOverUI : EventBasedUI
{
    [SerializeField] TextMeshProUGUI textMesh;

    [Header("References")]
    [SerializeField] GameStatistics statistics;

    public override void Show(){
        base.Show();
        textMesh.SetText(statistics.TotalKilled.ToString());
    }

    public void Restart(){
        SceneManager.LoadSceneAsync(0);
    }
}
