using UnityEngine;
using System.Collections;
using Toolbox;
using Toolbox.Pooling;
using Pathfinding;

public class Dummy : MonoBehaviour, ISpawn, IDespawn
{
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] GameObject deathFX;
    [SerializeField] GameObject coin;

    // Checks
    public bool IsMoving => target.IsSet && !health.IsDead;

    [Header("Components")]
    [SerializeField] TransformAnchor target;
    Transform Player => target.Value;
    [SerializeField] Animator animator;

    IAstarAI ai;
    Health health;

    #region Initialize
    void Awake(){
        ai = GetComponent<IAstarAI>();
        health = GetComponent<Health>();
    }

    public void OnSpawn(){
        health.Reset();
        ai.canMove = true;
        ai.maxSpeed = moveSpeed;
    }

    void OnEnable(){
        health.OnDie += Die;
        health.OnTakeDamage += TakeDamage;

        if (ai != null) ai.onSearchPath += SetAIdestination;
    }

    void OnDisable(){
        health.OnDie -= Die;
        health.OnTakeDamage -= TakeDamage;
        StopAllCoroutines();

        if (ai != null) ai.onSearchPath -= SetAIdestination;
    }
    #endregion

    void SetAIdestination(){
        if (target.IsSet && ai != null){
            ai.destination = Player.position;
        }
    }

    void Update()
    {
        animator.SetBool("IsMoving", IsMoving);
        SetAIdestination();
    }

    #region Health
    void TakeDamage(int damage){
        if (!health.IsDead){
            animator.SetTrigger("TakeDamage");
        }
    }

    void Die(){
        animator.SetTrigger("Die");
        ai.canMove = false;
        StartCoroutine(DoDie());
    }
    #endregion

    IEnumerator DoDie(){
        Pooler.Spawn(deathFX, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(.2f);
        Pooler.Despawn(gameObject);

    }

    public void OnDespawn(){
        DropLoot();
    }

    void DropLoot(){
        Pooler.Spawn(coin, transform.position, Quaternion.identity);
    }
}
