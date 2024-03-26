using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]

public class Character : MonoBehaviour, IPunObservable // 인터페이스(약속)
{
    public PhotonView PhotonView {  get; private set; } 

    public Stat Stat;

    private void Awake()
    {
        Stat.Init();
        PhotonView = GetComponent<PhotonView>();
        
        if (PhotonView.IsMine)
        {
            UI_CharacterStat.Instance.MyCharacter = this;
        }
    }

    // 데이터 동기화를 위해 데이터 전송 및 수신 기능을 가진 약속
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { 
        // stream(통로)은 서버에서 주고받을 데이터가 담겨있는 변수
        if(stream.IsWriting)        // 데이터를 전송하는 상황
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if(stream.IsReading)   // 데이터를 수신하는 상황
        {
            // 데이터를 전송한 순서와 똑같이 받은 데이터를 캐스팅해야된다.
            Vector3 recivedPosition = (Vector3)stream.ReceiveNext();
            Quaternion recivedRotation = (Quaternion)stream.ReceiveNext();

            if (!PhotonView.IsMine)
            {
                transform.position = recivedPosition;
                transform.rotation = recivedRotation;
            }
            
        }
        // info는 송수신 성공/실패 여부에 대한 메시지 담겨있다. 
    }
}