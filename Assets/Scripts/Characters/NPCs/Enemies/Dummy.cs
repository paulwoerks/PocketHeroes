using UnityEngine;
using System.Collections;
using Toolbox;
using Toolbox.Pooling;
using Toolbox.Events;
using Pathfinding;

public class Dummy : Character, ISpawn {
    [SerializeField] GameObject deathFX;
    [SerializeField] GameObject coin;
    // Checks
    public bool IsMoving => target.IsSet && !damagable.IsDead;

    [Header("Components")]
    [SerializeField] TransformAnchor target;
    Transform Target => target.Value;

    IAstarAI ai;

    #region Initialize
    public override void Awake(){
        base.Awake();
        ai = GetComponent<IAstarAI>();
        ai.maxSpeed = MoveSpeed;
    }

    public void OnSpawn(){
        Reset();
    }

    public override void Reset()
    {
        base.Reset();
        ai.canMove = true;
    }

    public override void OnEnable(){
        base.OnEnable();
        if (ai != null) ai.onSearchPath += SetAIdestination;
    }

    public override void OnDisable(){
        base.OnEnable();
        if (ai != null) 
            ai.onSearchPath -= SetAIdestination;
    }
    #endregion

    void SetAIdestination(){
        if (target.IsSet && ai != null)
            ai.destination = Target.position;
    }

    void Update()
    {
        animator.SetBool("IsMoving", IsMoving);
        SetAIdestination();
    }

    #region Health

    public override void Die(){
        base.Die();
        ai.canMove = false;
        StartCoroutine(DoDie());
    }
    #endregion

    IEnumerator DoDie(){
        Pooler.Spawn(deathFX, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(.2f);
        Pooler.Despawn(gameObject);
        statistics.OnKill();
        DropLoot();
    }

    void DropLoot(){
        Pooler.Spawn(coin, transform.position, Quaternion.identity);
    }
}
