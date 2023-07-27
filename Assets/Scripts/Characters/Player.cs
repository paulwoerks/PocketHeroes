using UnityEngine;
using Toolbox;

public class Player : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    JoystickReader joystick;
    [SerializeField] Animator anim;
    [SerializeField] GameObject audioC;

    [Header("Components")]
    [SerializeField] StompAttack stompAttack;
    [SerializeField] FrontSlash slashAttack;
    [SerializeField] TransformAnchor PlayerAnchor;
    Health health;
    public bool IsMoving => joystick.IsPressed && !health.IsDead;

    void Awake(){
        health = GetComponent<Health>();

        joystick = new("Movement");
        PlayerAnchor.Provide(transform);
        Reset();
    }

    void OnEnable(){
        health.OnDie += Die;
    }

    void OnDisable(){
        health.OnDie -= Die;
    }

    void Update(){
        if (!health.IsDead){
            Rotate();
            Move();

            if (Input.GetKeyDown(KeyCode.Q)){
                Attack();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.E)){
            if (!health.IsDead)
                health.TakeDamage(1);
            else{
                PlayerAnchor.Provide(transform);
                anim.SetTrigger("Revive");
                Reset();
            }
        }
    }

    void Move(){
        transform.position += transform.forward * joystick.Power
            * moveSpeed
            * Time.deltaTime;

        anim?.SetBool("isMoving", IsMoving);
        audioC.SetActive(IsMoving);
    }

    void Rotate(){
        if (!joystick.IsPressed || joystick.Direction3D(-45) == Vector3.zero)
            return;
        
        Quaternion targetRotation = Quaternion.LookRotation(joystick.Direction3D(-45));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void Attack(){
        anim?.SetTrigger("Attack_Strike");
    }

    void Die(){
        anim?.SetTrigger("Die");
        PlayerAnchor.Unset();
        stompAttack?.Deactivate();
        slashAttack?.Deactivate();
    }

    void Reset(){
        health.Reset();
        stompAttack?.Activate();
        slashAttack?.Activate();
    }
}