using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    // 목표: 일정 시간마다 아이템을 랜덤한 개수만큼 생성해서 흩뿌리고 싶다.
    // 필요 속성
    // - 시간 (일정 시간, 현재 시간, 랜덤 최소/최대 시간)
    private float _currentTime;
    private float _createTime;
    public float MinCreateTime = 10f;
    public float MaxCreateTime = 50f;

    // - 랜덤 개수 (최소/최대 개수, 확정 개수)
    public float _createCount;
    public int MinCreateCount = 10;
    public int MaxCreateCount = 30;

    // 생성한 아이템
    public List<ItemObject> _items = new List<ItemObject>();

    private void Start()
    {
        _createTime = 1f;
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        
        // 구현 순서
        // 1. 시간이 흐르다가
        _currentTime += Time.deltaTime;

        // 2. 생성할 시간이 되면
        if(_currentTime > _createTime)
        {
            // 아이템 오브젝트가 너무 많으면 아무것도 안한다..
            _items.RemoveAll(i=>i==null || i.isActiveAndEnabled == false); // 비활성화되었거나 삭제된 아이템을 리스트에서 제거
            if (_items.Count >= MaxCreateCount)
            {
                return;
            }

            // 3. 랜덤한 개수를 정하고
            _createCount = Random.Range(MinCreateCount, MaxCreateCount);
            Debug.Log(_createCount);

            // 4. 랜덤한 근처 위치에 생성한다.
            for (int i = 0; i < _createCount; i++)
            {
                Vector3 randomPosition = new Vector3(Random.Range(-10, 10), 1, Random.Range(-10, 10));
                randomPosition += transform.position;   // 랜덤 위치를 기준 위치로부터 결정하기 위함

                // 아이템 오브젝트를 생성하고 리스트에 추가
                int random = UnityEngine.Random.Range(0, 22);
                ItemType itemType = ItemType.ScoreStone30; // 기본 타입
                switch (random)
                {
                    case 9: // 랜덤숫자 '9'일때만
                        itemType = ItemType.ScoreStone100;
                        break;
                    case 8:
                        itemType = ItemType.ScoreStone50;
                        break;
                    case 7:
                        itemType = ItemType.StaminaPotion;
                        break;
                    case 6:
                        itemType = ItemType.HealthPotion;
                        break;
                }


                ItemObject itemObject = ItemObjectFactory.Instance.MasterCreate(itemType, randomPosition);
                _items.Add(itemObject);
                itemObject.transform.SetParent(transform);  //생성된 아이템은 스포너(transform)의 자식으로 설정됨
            }

            // 5. 생성할 시간을 다시 랜덤...
            _currentTime = 0f;
            _createTime = Random.Range(MinCreateTime, MaxCreateTime);
            Debug.Log(_createTime);
        }
    }
}
