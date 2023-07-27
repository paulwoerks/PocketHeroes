using System.Collections;
using UnityEngine;
using Toolbox;

public class StompAttack : MonoBehaviour
{
    public TransformGroup enemies;
    public int damage;
    public float cooldown;
    public float radius;
    
    Coroutine attackRoutine;

    public void Activate(){
        attackRoutine = StartCoroutine(CooldownTimer());
    }

    public void Deactivate(){
        StopCoroutine(attackRoutine);
    }

    void Perform(){
        Transform[] targets = enemies.GetInRange(transform.position, radius);
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
