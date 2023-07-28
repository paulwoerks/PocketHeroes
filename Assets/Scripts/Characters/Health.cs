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

    public bool IsDead => hp <= 0;

    public Action OnDie;
    public Action<int> OnTakeDamage;

    public void Start(){
        Reset();
    }

    public void Reset() => hp = maxHP;

    public void TakeDamage(int damage){
        if (damage <= 0 || IsDead)
            return;
        
        this.Log("Damage Taken.", debug);
        hp -= damage;

        OnTakeDamage?.Invoke(damage);

        // Is Dead
        if (IsDead) {
            hp = 0;
            OnDie?.Invoke();
            return;
        }
    }
}
