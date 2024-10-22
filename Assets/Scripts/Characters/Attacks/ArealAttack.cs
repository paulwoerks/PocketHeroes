using System.Collections;
using UnityEngine;
using Toolbox;
using Toolbox.Pooling;

public class ArealAttack : MonoBehaviour
{
    public TransformGroup enemies;
    public int damage;
    public float cooldown;
    public float radius;
    
    [SerializeField] GameObject FX_explosion;
    Coroutine attackRoutine;

    [SerializeField] Player player;

    void OnEnable(){
        if (player != null)
            player.OnAttacking += SetAttacking;
    }

    void OnDisable(){
        if (player != null)
            player.OnAttacking -= SetAttacking;
    }

    public void SetAttacking(bool isAttackMode){
        if (isAttackMode)
            attackRoutine = StartCoroutine(CooldownTimer());
        else
            StopCoroutine(attackRoutine);
    }

    void Perform(){
        Transform[] targets = enemies.GetInRange(transform.position, radius);
        Pooler.Spawn(FX_explosion, transform.position, transform.rotation);
        foreach (Transform target in targets){
            Damagable targetHealth = target.GetComponent<Damagable>();
            targetHealth.TakeDamage(damage);
        }
    }

    IEnumerator CooldownTimer(){
        while (true){
            yield return new WaitForSeconds(cooldown);
            Perform();
        }
    }
}
