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

    [PunRPC]
    private void Create(ItemType type, Vector3 position) // 함부로 접근 못함: PRIVATE
    {
        Vector3 dropPos = position + new Vector3(0, 0.5f, 0f) + Random.insideUnitSphere;
        PhotonNetwork.InstantiateRoomObject(type.ToString(), dropPos, Quaternion.identity); 
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
