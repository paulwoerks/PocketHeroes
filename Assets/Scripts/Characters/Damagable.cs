using UnityEngine;
using System;
using Toolbox;

public class Damagable : MonoBehaviour
{
    [SerializeField] bool debug;
    Character stats;
    int hp;
    public int HP => hp;
    public int MaxHP => stats.MaxHP;
    public bool IsDead => hp <= 0;

    // Behaviour
    public Action OnDie;
    public Action OnDodge;
    public Action<int> OnTakeDamage;
    public Action OnUpdateHealth;

    public void Provide(Character stats) => this.stats = stats;

    public void Reset(){
        hp = MaxHP;
        OnUpdateHealth?.Invoke();
    }

    public void TakeDamage(int damage){
        if (damage <= 0 || IsDead)
            return;

        if (CanDodge()){
            OnDodge?.Invoke();
            return;
        }
        
        this.Log("Damage Taken.", debug);
        hp -= damage;

        OnTakeDamage?.Invoke(damage);
        OnUpdateHealth?.Invoke();

        if (IsDead) {
            hp = 0;
            OnDie?.Invoke();
            return;
        }
    }
    bool CanDodge() => stats.Dodge > UnityEngine.Random.Range(0, 101);
}
