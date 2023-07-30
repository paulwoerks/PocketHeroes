using UnityEngine;
using System;
using Toolbox;
using Toolbox.Pooling;

public class Health : MonoBehaviour
{
    [SerializeField] bool debug;
    [SerializeField] int maxHP;
    [SerializeField] int hp;
    public int HP => hp;
    public int HPmax => maxHP;

    public bool IsDead => hp <= 0;

    public Action OnDie;
    public Action<int> OnTakeDamage;
    public Action OnUpdateHealth;

    public void Start(){
        Reset();
    }

    public void Reset(){
        hp = maxHP;
        OnUpdateHealth?.Invoke();     
    } 

    public void TakeDamage(int damage){
        if (damage <= 0 || IsDead)
            return;
        
        this.Log("Damage Taken.", debug);
        hp -= damage;

        OnTakeDamage?.Invoke(damage);
        OnUpdateHealth?.Invoke();
        // Is Dead
        if (IsDead) {
            hp = 0;
            OnDie?.Invoke();
            return;
        }
    }
}
