using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

[RequireComponent(typeof(Animator))]

public class CharacterAttackAbility : CharacterAbility
{
    // SOLID 법칙: 객체지향 5가지 법칙
    // 1. 단일 책임 원칙 (가장 단순하지만 꼭 지켜야 하는 원칙)
    // - 클래스는 단 한 개의 책임을 가져야 한다.
    // - 클래스를 변경하는 이유는 단 하나여야 한다.
    // - 이를 지키지 않으면 한 책임 변경에 의해 다른 책임과 관련된 코드도 영향을 미칠 수 있어서
    //      -> 유지보수가 매우 어렵다. 
    // 준수 전략
    // - 기존의 클래스로 해결할 수 없다면 새로운 클래스를 구현

    private Animator _animator;
    private float _attackTimer = 0;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!Owner.PhotonView.IsMine)
        {
            return;
        }

        _attackTimer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && _attackTimer > Owner.Stat.AttackCoolTime)
        {
            _attackTimer = 0f;

            Owner.PhotonView.RPC(nameof(PlayAttackAnimation), RpcTarget.All, Random.Range(1,4));
            // RpcTarget.All : 모두에게
            // RpcTarget.Others : 나 자신을 제외하고 모두에게
            // RpcTarget.Master : 방장에게만
        }

    }

    [PunRPC] // RPC : 원격 프로시저 호출(remote procedure call)
    public void PlayAttackAnimation(int index)
    {
        _animator.SetTrigger($"Attack0{index}");
    }
}
