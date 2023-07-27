using UnityEngine;
using System;
using System.Collections;
using Toolbox;
using Toolbox.Pooling;

public class Dummy : MonoBehaviour, ISpawn, IDespawn
{
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float rotationSpeed = 1f;

    [SerializeField] GameObject deathFX;
    [SerializeField] GameObject coin;

    // Checks
    public bool IsMoving => PlayerAnchor.IsSet && !health.IsDead;

    [Header("Components")]
    [SerializeField] TransformAnchor PlayerAnchor;
    [SerializeField] Animator animator;
    Health health;

    #region Initialize
    void Awake(){
        health = GetComponent<Health>();
    }

    public void OnSpawn(){
        health.Reset();
    }

    void OnEnable(){
        health.OnDie += Die;
        health.OnTakeDamage += TakeDamage;
    }

    void OnDisable(){
        health.OnDie -= Die;
        health.OnTakeDamage -= TakeDamage;
        StopAllCoroutines();
    }
    #endregion

    void Update()
    {
        if (IsMoving){
            Rotate();
            Move();
        }
        animator.SetBool("IsMoving", IsMoving);
    }

    #region Move
    void Rotate(){
        Quaternion targetRotation = Quaternion.LookRotation(PlayerAnchor.Value.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void Move(){
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
    #endregion

    #region Health
    void TakeDamage(){
        animator.SetTrigger("TakeDamage");
    }

    void Die(){
        animator.SetTrigger("Die");
        StartCoroutine(DoDie());
    }
    #endregion

    IEnumerator DoDie(){
        yield return new WaitForSeconds(1.2f);
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
