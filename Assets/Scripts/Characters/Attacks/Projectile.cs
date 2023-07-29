using UnityEngine;
using Toolbox.Pooling;

public class Projectile : MonoBehaviour
{
    float projectileSpeed;
    int damage;
    [SerializeField] LayerMask collisionLayers;

    Vector3 previousPosition;

    [SerializeField] GameObject impactFX;
    [SerializeField] GameObject explosionFX;

    public void SetArrow(float projectileSpeed, int damage){
        this.projectileSpeed = projectileSpeed;
        this.damage = damage;
    }

    void Update(){
        Move();
        CheckCollision(previousPosition);
        previousPosition = transform.position;
    }

    void Move(){
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }

    void CheckCollision(Vector3 prevPos){
        RaycastHit hit;
        Vector3 direction = transform.position - prevPos;
        Ray ray = new Ray(prevPos, direction);
        float dist = Vector3.Distance(transform.position, prevPos);

        if (Physics.Raycast(ray, out hit, dist, collisionLayers)){
            transform.position = hit.point;
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            Vector3 pos = hit.point;

            Pooler.Spawn(impactFX, pos, rot);
            if (explosionFX != null)
                Pooler.Spawn(explosionFX, pos, rot);

            GameObject target = hit.transform.gameObject;
            if (target.CompareTag("Enemy")){
                target.GetComponent<Health>().TakeDamage(damage);
            }

            Pooler.Despawn(gameObject);
        }
    }
}
