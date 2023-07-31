using UnityEngine;
public class Character : MonoBehaviour
{
    public float MoveSpeed => BaseStats.moveSpeed + moveSpeed;
    float moveSpeed;
    public int MaxHP => BaseStats.hp + hpMod;
    public int Dodge => BaseStats.dodge + dodgeMod;
    int hpMod;
    int dodgeMod;

    [SerializeField] CharacterBaseStats BaseStats;
    public GameStatistics statistics;

    public Damagable damagable;
    public Animator animator;

    public virtual void Awake(){
        damagable?.Provide(this);
    }
    
    public virtual void OnEnable(){
        if (damagable != null){
            damagable.OnTakeDamage += TakeDamage;
            damagable.OnDie += Die;
        }
    }

    public virtual void OnDisable(){
        if (damagable != null){
            damagable.OnTakeDamage -= TakeDamage;
            damagable.OnDie -= Die;
        }
        StopAllCoroutines();
    }

    public virtual void TakeDamage(int damage){
        if (damagable != null && damagable.IsDead)
            return;
        animator?.SetTrigger("TakeDamage");
    }

    public virtual void Die(){
        animator?.SetTrigger("Die");
    }

    public virtual void Reset(){
        damagable.Reset();
    }
}
