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

    public void Activate(){
        attackRoutine = StartCoroutine(CooldownTimer());
    }

    public void Deactivate(){
        StopCoroutine(attackRoutine);
    }

    Vector3 attackBoxSize = new Vector3(5f, 5f, 10f);
    void Perform(){
        // Spawn thing;
        Pooler.Spawn(VFX, transform.position, transform.rotation);
        List<Health> enemies = GetEnemiesInBox();
        foreach (Health health in enemies){
            health.TakeDamage(damage);
        }
    }

    public List<Health> GetEnemiesInBox() {
        List<Health> hitEnemies = new List<Health>();

        // Mittelpunkt der Box
        Vector3 boxCenter = transform.position + transform.forward * attackBoxSize.z / 2;

        // BoxCast ausführen
        RaycastHit[] hits = Physics.BoxCastAll(boxCenter, attackBoxSize / 2, transform.forward, transform.rotation, attackBoxSize.z, enemyLayer);
        foreach (RaycastHit hit in hits) {
            // Überprüfen, ob das getroffene Objekt eine Health-Komponente hat
            Health health = hit.transform.GetComponent<Health>();
            if (health != null) {
                hitEnemies.Add(health);
            }
        }

        //OnDrawGizmos();

        return hitEnemies;
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
}
