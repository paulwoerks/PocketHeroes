using System.Collections;
using UnityEngine;
using Toolbox;
using Toolbox.Pooling;

public class StompAttack : MonoBehaviour
{
    public TransformGroup enemies;
    public int damage;
    public float cooldown;
    public float radius;
    
    [SerializeField] GameObject FX_explosion;
    Coroutine attackRoutine;

    [SerializeField] Player player;

    void OnEnable(){
        player.OnAttacking += SetAttacking;
    }

    void OnDisable(){
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
            Health targetHealth = target.GetComponent<Health>();
            targetHealth.TakeDamage(damage);
        }
    }

    IEnumerator CooldownTimer(){
        while (true){
            Perform();
            yield return new WaitForSeconds(cooldown);
        }
    }
}
