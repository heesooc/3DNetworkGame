using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CharacterController))] // rigidbody 쓰지 않고 가능하다는 뜻
[RequireComponent(typeof(Animator))]

public class CharacterMoveAbility : MonoBehaviour
{

    private CharacterController _characterController;
    private Animator _animator;

    public float MoveSpeed = 2f;
    private float VerticalSpeed = 0f;
    private float _gravity = -9.8f;

    // 목표: [W], [A], [S], [D] 및 방향키를 누르면 캐릭터를 그 방향으로 이동시키고 싶다.

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 순서
        // 1. 사용자의 키보드 입력을 받는다.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2. "캐릭터가 바라보는 방향을 기준으로 방향을 설정한다.
        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();
        dir = Camera.main.transform.TransformDirection(dir);


        _animator.SetFloat("Move", dir.magnitude);

        // 4. 중력 적용하세요.
        if (!_characterController.isGrounded)
        {
            VerticalSpeed += _gravity * Time.deltaTime;
        }
        else
        {
            VerticalSpeed = 0; // 지면에 닿아 있을 경우 수직 속도를 0으로 리셋
        }


        // 3. 이동속도에 따라 그 방향으로 이동한다. 
        dir.y = VerticalSpeed;
        _characterController.Move(dir * MoveSpeed * Time.deltaTime);


    }
}