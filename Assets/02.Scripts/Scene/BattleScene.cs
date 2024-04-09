using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScene : MonoBehaviourPunCallbacks
{
    public static BattleScene Instance { get; private set; }

    public List<Transform> SpawnPoints;

    public bool _init = false;

    private void Awake()
    {
        Instance = this;
    }
    public Vector3 GetRandomSpawnPoint()
    {
        int randomIndex = Random.Range(0, SpawnPoints.Count);
        return SpawnPoints[randomIndex].position;
    }

    private void Start()
    {
        if (!_init)
        {
            Init();
        }
    }

    public override void OnJoinedRoom()
    {
        if (!_init)
        {
            Init();
        }
    }

    public void Init()
    {
        _init = true;

        // Character_Male <- 리소스에 넣어줄 이름
        // Character_Female
        PhotonNetwork.Instantiate($"Character_{UI_Lobby.SelectedCharacterType}", Vector3.zero, Quaternion.identity);

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        GameObject[] points = GameObject.FindGameObjectsWithTag("BearSpawnPoint");
        foreach (GameObject point in points)
        {
            PhotonNetwork.InstantiateRoomObject("PolarBear", point.transform.position, Quaternion.identity);
        }
    }


}
