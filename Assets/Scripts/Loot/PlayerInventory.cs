using UnityEngine;
using Toolbox;
using Toolbox.Events;

[CreateAssetMenu(fileName = "Player Inventory", menuName = "Loot/Inventory/Player Inventory", order = 2)]
public class PlayerInventory : SO
{
    [SerializeField] int coins;

    [Header("Listening to ...")]
    [SerializeField] IntChannelSO OnGetCoins;
    [Header("Broadcasting on ...")]
    [SerializeField] IntChannelSO OnUpdateCoins;

    void Start(){
        OnUpdateCoins?.Invoke(coins);
    }

    void OnEnable(){
        OnGetCoins?.Subscribe(ReceiveCoins, this);
    }

    void OnDisable(){
        OnGetCoins?.Unsubscribe(ReceiveCoins, this);
        Reset();
    } 

    void ReceiveCoins(int coins){
        this.coins += coins;
        OnUpdateCoins?.Invoke(this.coins);
    }

    void Reset(){
        coins = 0;
    }
}
