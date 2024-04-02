using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CharacterController))] // rigidbody 쓰지 않고 가능하다는 뜻
[RequireComponent(typeof(Animator))]

public class CharacterMoveAbility : CharacterAbility
{
    public bool IsJumping => !_characterController.isGrounded;

    // 목표: [W], [A], [S], [D] 및 방향키를 누르면 캐릭터를 그 방향으로 이동시키고 싶다.

    private CharacterController _characterController;
    private Animator _animator;

    private float _yVelocity = 0f;
    private float _gravity = -9.8f;


    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

    }

    private void Update()
    {
        if(Owner.State == State.Death || !Owner.PhotonView.IsMine)
        {
            return;
        }

        // 순서
        // 1. 사용자의 키보드 입력을 받는다.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2. "캐릭터가 바라보는 방향을 기준으로 방향을 설정한다.
        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();
        dir = Camera.main.transform.TransformDirection(dir);

        _animator.SetFloat("Move", dir.magnitude);

        // 3. 중력 적용하세요.
        _yVelocity += _gravity * Time.deltaTime;
        dir.y = _yVelocity;


        // 스태미나 적용
        float speed = Owner.Stat.MoveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(Owner.Stat.Stamina > 0)
            {
                speed = Owner.Stat.RunSpeed;
                Owner.Stat.Stamina -= Time.deltaTime * Owner.Stat.RunConsumeStamina;
            }
            else
            {
                speed = Owner.Stat.MoveSpeed;
            }
        }
        else
        {
            Owner.Stat.Stamina += Time.deltaTime * Owner.Stat.RecoveryStamina;
        }

        Owner.Stat.Stamina = Mathf.Clamp(Owner.Stat.Stamina, 0, Owner.Stat.MaxStamina);


        // 4. 이동속도에 따라 그 방향으로 이동한다. 

        _characterController.Move(dir * speed * Time.deltaTime);

        // 5. 점프 적용하기
        bool haveJumpStamina = Owner.Stat.Stamina >= Owner.Stat.JumpConsumeStamina;
        if (haveJumpStamina && Input.GetKeyDown(KeyCode.Space) && _characterController.isGrounded) //&& (Owner.Stat.JumpRemainCount > 0)
        {
            /*Debug.Log("점프");
            Owner.Stat.JumpRemainCount--;*/

            Owner.Stat.Stamina -= Owner.Stat.JumpConsumeStamina;
            _yVelocity = Owner.Stat.JumpPower;
        }
    }

    public void Teleport(Vector3 position)
    {
        _characterController.enabled = false;

        transform.position = position;

        _characterController.enabled = true;
    }
}
