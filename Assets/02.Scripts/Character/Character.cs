using Cinemachine;
using Photon.Pun;
using UnityEngine;
using System.Collections;
using Photon.Realtime;

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

    public float Score;
    private int _halfScore;

    private void Awake()
    {
        Stat.Init();
        PhotonView = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
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

        if (!PhotonView.IsMine)
        {
            return;
        }
        SetRandomPositionAndRotation();

        /*[해쉬테이블] // 효율적인 검색과 삽입 연산을 위해 설계된 자료구조
         ㄴ int score     = 0;
         ㄴ int KillCount = 0;
         ㄴ. 캐릭터.커스텀 프로퍼티 = 해쉬테이블<string, object>*/
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add("Score", 0);      // hashtable.Add("키", 값); //여기서 키(string 타입)는 데이터를 식별하는 데 사용되며, 값(object 타입)은 저장하려는 실제 데이터
        hashtable.Add("KillCount", 0);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable); //현재 로컬 플레이어의 CustomProperties를 방금 만든 hashtable로 설정함

    }

    [PunRPC]
    public void AddPropertyIntValue(string key, int value)
    {
        // 현재 로컬 플레이어에 할당된 커스텀 프로퍼티들을 담고 있는 해시테이블을 가져옴
        ExitGames.Client.Photon.Hashtable myHashtable = PhotonNetwork.LocalPlayer.CustomProperties;
        // 'Score' 키에 해당하는 값을 가져옴 = 형 변환을 통해 object-> int 타입으로 변환 + 현재 점수에 새 점수 더함
        myHashtable[key] = (int)myHashtable[key] + value;
        // 변경된 해시테이블을 다시 플레이어의 커스텀 프로퍼티로 설정
        PhotonNetwork.LocalPlayer.SetCustomProperties(myHashtable);
       GetComponent<CharacterAttackAbility>().RefreshWeaponScale();
    }

    public void SetPropertyIntValue(string key, int value)
    {
        ExitGames.Client.Photon.Hashtable myHashtable = PhotonNetwork.LocalPlayer.CustomProperties;
        myHashtable[key] = value;
        PhotonNetwork.LocalPlayer.SetCustomProperties(myHashtable);
        GetComponent<CharacterAttackAbility>().RefreshWeaponScale();
    }

    public int GetPropertyIntValue(string key)
    {
        ExitGames.Client.Photon.Hashtable myHashtable = PhotonNetwork.LocalPlayer.CustomProperties;
        return (int)myHashtable[key];
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
        _halfScore = GetPropertyIntValue("Score") / 2;
        SetPropertyIntValue("Score", 0);

        if (actorNumber >= 0)
        {
            string nickname = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).NickName;
            string logMessage = $"\n{nickname}님이 {PhotonView.Owner.NickName}을 처치하였습니다.";
            PhotonView.RPC(nameof(AddLog), RpcTarget.All, logMessage);

            Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
            PhotonView.RPC(nameof(AddPropertyIntValue), targetPlayer, "Score", _halfScore);

            PhotonView.RPC(nameof(AddPropertyIntValue), targetPlayer, "KillCount", 1);
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
            DropItems();
            StartCoroutine(Death_Coroutine());
        }
    }

    [PunRPC]
    private void DropItems()
    {
        // 팩토리패턴: 객체 생성과 사용 로직을 분리해서 캡슐화하는 패턴
        int randomValue = UnityEngine.Random.Range(0, 100);
        if (randomValue > 30)      // 70%
        {
            Debug.Log(randomValue);
            int randomCount = _halfScore / 30;
            for (int i = 0; i < randomCount; ++i)
            {
                ItemObjectFactory.Instance.RequestCreate(ItemType.ScoreStone100, transform.position);
            }
        }
        else if (randomValue > 10)  // 20%
        {
            Debug.Log(randomValue);
            ItemObjectFactory.Instance.RequestCreate(ItemType.HealthPotion, transform.position);
        }
        else                        // 10%
        {
            Debug.Log(randomValue);
            ItemObjectFactory.Instance.RequestCreate(ItemType.StaminaPotion, transform.position);
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
