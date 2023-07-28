using System.Collections;
using UnityEngine;
using Toolbox;
using Toolbox.Pooling;

public class RangedAttack : MonoBehaviour
{
    public int damage;
    public float attackRange;
    public float cooldownTime;
    public float projectileSpeed;


    [SerializeField] TransformGroup targets;
    [SerializeField] Player player;
    
    Coroutine AttackLoop;

    [SerializeField] GameObject projectilePrefab;

    void OnEnable(){
        if (player != null)
            player.OnAttacking += SetAttackMode;
    }

    void OnDisable(){
        if (player != null)
            player.OnAttacking -= SetAttackMode;
    }

    void SetAttackMode(bool isAttacking){
        if (isAttacking)
            AttackLoop = StartCoroutine(CooldownTimer());
    }

    void Perform(){
        Transform target = targets.GetClosest(transform.position, attackRange);
        if (target == null)
            return;

        Quaternion rotation = Quaternion.Euler(transform.position - target.position);
        GameObject projectile = Pooler.Spawn(projectilePrefab, transform.position, rotation);
        // Set Data to Projectile
        Arrow arrow = projectile.GetComponent<Arrow>();
        arrow.SetArrow(projectileSpeed, damage);
    }

    IEnumerator CooldownTimer(){
        while (true){
            yield return new WaitForSeconds(cooldownTime);
            Perform();
        }
    }
}
