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

    [SerializeField] Transform muzzleLocator;
    [SerializeField] Transform projectileLocator;

    [SerializeField] TransformGroup targets;
    [SerializeField] Player player;
    
    Coroutine AttackLoop;

    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject muzzleFX;

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
        else
            StopCoroutine(AttackLoop);
    }

    void Perform(){

        Transform target = targets.GetClosest(transform.position, attackRange);
        if (target == null)
            return;

        transform.LookAt(target.position, Vector3.up); 

        Quaternion rotation = projectileLocator.rotation;
        
        if (muzzleFX != null)
            Pooler.Spawn(muzzleFX, muzzleLocator.position, rotation);
        
        GameObject projectile = Pooler.Spawn(projectilePrefab, projectileLocator.position, rotation);
        Projectile arrow = projectile.GetComponent<Projectile>();
        arrow.SetArrow(projectileSpeed, damage);
    }

    IEnumerator CooldownTimer(){
        while (true){
            yield return new WaitForSeconds(cooldownTime);
            Perform();
        }
    }
}
