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

    private void Start()
    {
        Debug.LogError($"viewID: {photonView.ViewID}");
    }

    public ItemObject MasterCreate(ItemType type, Vector3 position)
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("[MasterCreate] 마스터 클라이언트만 호출할 수 있습니다.");
            return null;
        }

        return Create(type, position);
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
    private ItemObject Create(ItemType type, Vector3 position) // 함부로 접근 못함: PRIVATE
    {
        Vector3 dropPos = position + new Vector3(0, 0.5f, 0f) + Random.insideUnitSphere;
        GameObject gameObject = PhotonNetwork.InstantiateRoomObject(type.ToString(), dropPos, Quaternion.identity);
        // 이유) 플레이어 나가면 아이템 사라짐 -> 방장만 관리할 수 있음

        return gameObject.GetComponent<ItemObject>();
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
        GameObject objectToDelete = PhotonView.Find(viewID)?.gameObject;
        if (objectToDelete != null)
        {
            PhotonNetwork.Destroy(objectToDelete);
        }
    }
}
