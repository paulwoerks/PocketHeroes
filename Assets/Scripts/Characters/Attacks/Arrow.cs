using UnityEngine;
using Toolbox.Pooling;

public class Arrow : MonoBehaviour
{
    float speed;
    int damage;

    public void SetArrow(float speed, int damage){
        this.speed = speed;
        this.damage = damage;
    }

    void Update(){
        Move();
    }

    void Move(){
        transform.position += transform.forward * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("Enemy")){
            other.gameObject.GetComponent<Health>().TakeDamage(damage);
        }
    }

    void Collide(){
        Pooler.Despawn(gameObject);
    }
}
