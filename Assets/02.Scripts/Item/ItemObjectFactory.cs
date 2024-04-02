using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class ItemObjectFactory : MonoBehaviourPun
{
    public static ItemObjectFactory Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void RequestCreate(ItemType type, Vector3 position)
    {
        if(PhotonNetwork.IsMasterClient) 
        {
            Create(type, position);
        }
        else
        {
            photonView.RPC(nameof(Create), RpcTarget.MasterClient, type, position);
        }
    }

    public void CreatePercent(Vector3 position)
    {
        int percentage = Random.Range(0, 100);
        if(percentage <= 10)
        {
            Create(ItemType.StaminaPotion, position);
            Debug.Log(percentage);
        }
        else if(percentage <= 20)
        {
            Create(ItemType.HealthPotion, position);
            Debug.Log(percentage);
        }
        else
        {
            Create(ItemType.ScorePotion, position);
            Debug.Log(percentage);
        }
    }

    [PunRPC]
    private void Create(ItemType type, Vector3 position) // 함부로 접근 못함: PRIVATE
    {
        if(type == ItemType.ScorePotion)
        {
            // 3-5개 개수 랜덤 생성
            int itemCount = Random.Range(3, 6);
            for (int i = 0; i < itemCount; i++)
            {
                Vector3 dropPoses = position + new Vector3(0, 0.5f, 0f) + Random.insideUnitSphere * 0.5f;
                PhotonNetwork.InstantiateRoomObject(type.ToString(), dropPoses, Quaternion.identity);
            }
        }
        else
        {
            Vector3 dropPos = position + new Vector3(0, 0.5f, 0f) + Random.insideUnitSphere;
            PhotonNetwork.InstantiateRoomObject(type.ToString(), dropPos, Quaternion.identity);
        }
        // 이유) 플레이어 나가면 아이템 사라짐 -> 방장만 관리할 수 있음
    }

    public void RequestDelete(int viewID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Delete(viewID);
        }
        else
        {
            photonView.RPC(nameof(Delete), RpcTarget.MasterClient, viewID);
        }
    }

    [PunRPC]
    private void Delete(int viewID)
    {
        GameObject objectToDelete = PhotonView.Find(viewID).gameObject;
        if (objectToDelete != null)
        {
            PhotonNetwork.Destroy(objectToDelete);
        }
    }
}
