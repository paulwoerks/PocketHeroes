using UnityEngine;
using Toolbox;

[CreateAssetMenu(fileName = "GameStatisticsSO", menuName = "Game/Statistics", order = 2)]
public class GameStatistics : SO
{
    [SerializeField] int killedTotal;
    public int TotalKilled => killedTotal;

    void OnEnable(){
        Reset();
    }

    void Reset(){
        killedTotal = 0;
    }

    public void OnKill(){
        killedTotal += 1;
    }
}
