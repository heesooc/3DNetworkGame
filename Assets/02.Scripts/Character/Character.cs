using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]
[RequireComponent(typeof(CharacterShakeAbility))]
[RequireComponent(typeof(Animator))]

public class Character : MonoBehaviour, IPunObservable, IDamaged // 인터페이스(약속)
{
    public PhotonView PhotonView {  get; private set; } 

    public Stat Stat;
    public State State { get; private set; } = State.Live;
    private Animator _animator;

    private Vector3 _recivedPosition;
    private Quaternion _recivedRotation;

    private void Awake()
    {
        Stat.Init();
        PhotonView = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();

        if (PhotonView.IsMine)
        {
            UI_CharacterStat.Instance.MyCharacter = this;

            MinimapCamera minimapCamera = FindObjectOfType<MinimapCamera>();

            // 미니맵 카메라의 타겟으로 이 플레이어 설정
            if (minimapCamera != null)
            {
                minimapCamera.SetTarget(transform);
            }
        }
    }

    private void Start()
    {
        SetRandomPositionAndRotation();
    }

    private void Update()
    {
        if (!PhotonView.IsMine)
        {
            //transform.position = Vector3.Lerp(transform.position, _recivedPosition, Time.deltaTime * 20f);
            //transform.rotation = Quaternion.Slerp(transform.rotation, _recivedRotation, Time.deltaTime * 20f);
        }

        /*if (transform.position.y < -20) // 예시로 -20을 사용
        {
            StartCoroutine(Death_Coroutine());
        }*/
    }

    // 데이터 동기화를 위해 데이터 전송 및 수신 기능을 가진 약속
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { 
        // stream(통로)은 서버에서 주고받을 데이터가 담겨있는 변수
        if(stream.IsWriting)        // 데이터를 전송하는 상황
        {
            /*stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);*/
            stream.SendNext(Stat.Health);
            stream.SendNext(Stat.Stamina);
        }
        else if(stream.IsReading)   // 데이터를 수신하는 상황
        {
            // 데이터를 전송한 순서와 똑같이 받은 데이터를 캐스팅해야된다.
            /*_recivedPosition = (Vector3)stream.ReceiveNext();
            _recivedRotation = (Quaternion)stream.ReceiveNext();*/
            Stat.Health = (int)stream.ReceiveNext();
            Stat.Stamina = (float)stream.ReceiveNext();
        }
        // info는 송수신 성공/실패 여부에 대한 메시지 담겨있다. 
    }

    [PunRPC]
    public void AddLog(string logMessage)
    {
        UI_RoomInfo.Instance.AddLog(logMessage);
    }

    [PunRPC]
    public void Damaged(int damage, int actorNumber)
    {
        if(State == State.Death)
        {
            return;
        }
        Stat.Health -= damage;
        if (Stat.Health <= 0)
        {
            if (PhotonView.IsMine)
            {
                OnDeath(actorNumber);
            }
            /* Death();*/
            PhotonView.RPC(nameof(Death), RpcTarget.All); // Death 함수를 호출
        }

        GetComponent<CharacterShakeAbility>().Shake();


        if (PhotonView.IsMine) 
        {
            OnDamagedMine();
        }
    }

    private void OnDeath(int actorNumber)
    {
        if (actorNumber >= 0)
        {
            string nickname = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).NickName;
            string logMessage = $"\n{nickname}님이 {PhotonView.Owner.NickName}을 처치하였습니다.";
            PhotonView.RPC(nameof(AddLog), RpcTarget.All, logMessage);
        }
        else
        {
            string logMessage = $"\n{PhotonView.Owner.NickName}이 운명을 다했습니다.";
            PhotonView.RPC(nameof(AddLog), RpcTarget.All, logMessage);
        }
    }


    private void OnDamagedMine()
    {
        // 카메라 흔들기 위해 Impulse를 발생시킨다.
        CinemachineImpulseSource impulseSource;
        if (TryGetComponent<CinemachineImpulseSource>(out impulseSource))
        {
            float strength = 0.2f;
            impulseSource.GenerateImpulseWithVelocity(Random.insideUnitSphere.normalized * strength);
        }

        UI_DamagedEffect.Instance.Show(0.4f);
    }

    [PunRPC]
    private void Death()
    {
        if (State == State.Death)
        {
            return;
        }

        State = State.Death;

        GetComponent<Animator>().SetTrigger($"Die");
        GetComponent<CharacterAttackAbility>().InActiveCollider();

        // 죽고나서 5초후 리스폰
        if(PhotonView.IsMine)
        {
            // 팩토리패턴: 객체 생성과 사용 로직을 분리해서 캡슐화하는 패턴
            ItemObjectFactory.Instance.RequestCreate(ItemType.HealthPotion, transform.position);
            ItemObjectFactory.Instance.RequestCreate(ItemType.StaminaPotion, transform.position);

            StartCoroutine(Death_Coroutine());
        }
    }


    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(5f);

        SetRandomPositionAndRotation();
        PhotonView.RPC(nameof(Live), RpcTarget.All);
    }

    private void SetRandomPositionAndRotation()
    {
        Vector3 spawnPoint = BattleScene.Instance.GetRandomSpawnPoint();
        GetComponent<CharacterMoveAbility>().Teleport(spawnPoint);
        GetComponent<CharacterRotateAbility>().SetRandomRotation();
    }

    [PunRPC]
    private void Live()
    {
        State = State.Live;
        Stat.Init();
        GetComponent<Animator>().SetTrigger("Live");
    }
}
