using System.Collections;
using UnityEngine;
using Toolbox;

public class DummyAttack : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] float cooldown = 2f;
    [SerializeField] TransformAnchor playerAnchor;
    Health playerHealth;

    bool canAttack;

    void OnEnable(){
        canAttack = true;
    }

    void OnDisable(){
        StopAllCoroutines();
    }

    void Update(){
        if (canAttack){
            TryAttack();
        }
    }

    void TryAttack(){
        if (!playerAnchor.IsSet)
            return;
            
        if (Vector3.Distance(playerAnchor.Value.position, transform.position) <= attackRange){
            playerAnchor.Value.GetComponent<Health>().TakeDamage(damage);
            canAttack = false;
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown(){
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}
