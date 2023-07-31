using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;
using Toolbox.Pooling;

public class FrontSlash : MonoBehaviour
{
    [SerializeField] LayerMask enemyLayer;
    public int damage;
    public float cooldown;
    public float radius;
    
    Coroutine attackRoutine;

    [SerializeField] Animator animator;

    [SerializeField] GameObject VFX;

    List<GameObject> inRange = new();

    [SerializeField] Player player;

    void OnEnable(){
        if (player != null)
            player.OnAttacking += SetAttacking;
    }

    void OnDisable(){
        if (player != null)
            player.OnAttacking -= SetAttacking;
    }

    public void SetAttacking(bool isAttacking){
        if (isAttacking)
            attackRoutine = StartCoroutine(CooldownTimer());
        else
            StopCoroutine(attackRoutine);
    }

    Vector3 attackBoxSize = new Vector3(5f, 5f, 10f);
    void Perform(){
        // Spawn thing;
        Pooler.Spawn(VFX, transform.position, transform.rotation);

        foreach (GameObject enemy in inRange){
            enemy.GetComponent<Damagable>().TakeDamage(damage);
        }
    }


    void OnDrawGizmos() {
        Gizmos.color = Color.red;

        Vector3 boxCenter = transform.position + transform.forward * attackBoxSize.z / 2;
        Gizmos.DrawWireCube(boxCenter, attackBoxSize);
    }

    IEnumerator CooldownTimer(){
        float animationDelay = 0.35f;
        while (true){
            yield return new WaitForSeconds(cooldown - animationDelay);
            animator.Play("Attack_Slash");
            yield return new WaitForSeconds(animationDelay);
            Perform();
        }
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.layer.Equals(enemyLayer)){
            inRange.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other){
        if (other.gameObject.layer.Equals(enemyLayer)){
            inRange.Remove(other.gameObject);
        }
    }
}
