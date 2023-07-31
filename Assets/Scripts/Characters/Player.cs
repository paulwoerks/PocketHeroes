using UnityEngine;
using Toolbox;
using Toolbox.Events;
using System;

public class Player : Character {
    public float rotationSpeed = 5f;
    JoystickReader joystick;
    [SerializeField] TransformAnchor PlayerAnchor;
    public Action<bool> OnAttacking;

    [Header("Broadcasting on ...")]
    [SerializeField] VoidChannelSO OnPlayerDie;
    public bool IsMoving => joystick.IsPressed && !damagable.IsDead;

    public override void Awake(){
        base.Awake();
        joystick = new("Movement");
        PlayerAnchor.Provide(transform);
    }

    void Start(){
        Reset();
    }

    void Update(){
        if (!damagable.IsDead){
            Rotate();
            Move();

            if (Input.GetKeyDown(KeyCode.Q)){
                Attack();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.E)){
            if (!damagable.IsDead)
                damagable.TakeDamage(1);
            else{
                PlayerAnchor.Provide(transform);
                animator.SetTrigger("Revive");
                Reset();
            }
        }
    }

    void Move(){
        transform.position += transform.forward * joystick.Power
            * MoveSpeed
            * Time.deltaTime;

        animator?.SetBool("isMoving", IsMoving);
    }

    void Rotate(){
        if (!joystick.IsPressed || joystick.Direction3D(-45) == Vector3.zero)
            return;
        
        Quaternion targetRotation = Quaternion.LookRotation(joystick.Direction3D(-45));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void Attack(){
        animator?.SetTrigger("Attack_Strike");
    }

    public override void Die(){
        base.Die();
        PlayerAnchor.Unset();
        OnAttacking?.Invoke(false);
        OnPlayerDie.Invoke();
    }

    public override void Reset(){
        base.Reset();
        OnAttacking?.Invoke(true);
    }
}
