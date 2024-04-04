using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;

public class UI_PlayerRanking : MonoBehaviourPunCallbacks
{
    public List<UI_PlayerRankingSlot> Slots; // 1 ~ 5등
    public UI_PlayerRankingSlot MySlot;      // 내 정보

    // 새로운 플레이어가 룸에 입장했을 때 호출되는 콜백 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Refresh();
    }
    // 플레이어가 룸에서 퇴장했을 때 호출되는 콜백 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Refresh();
    }

    // 플레이어의 커스텀 프로퍼티가 변경되면 호출되는 함수
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Refresh();
    }

    public override void OnJoinedRoom()
    {
        Refresh();
    }

    private void Refresh()
    {
        /* 딕셔너리 */
        /*
         
         Dictionary<TKey,TValue>
         이 딕셔너리는 플레이어들의 정보를 저장하는데 사용됨.
         - 키(Key): 각 플레이어의 고유 ID(정수 타입)
         - 값(Value): Player 객체 / Player 객체는 다양한 정보(닉네임, 커스텀 속성..)를 포함함

        */

        // 현재 방에 있는 모든 플레이어의 정보를 가져옴
        Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;


        // 리스트로 변환 (플레이어 정보). 순서대로 접근하기 용이하게 함
        List<Player> playerList = players.Values.ToList();
        playerList.RemoveAll(player  => !player.CustomProperties.ContainsKey("Score")); // 요소 검색
        playerList.Sort((player1, player2) => // 정렬  // "=>": 람다식
        {
            int player1Score = (int)player1.CustomProperties["Score"];  // 커스텀 프로퍼티" 빈번하지 않은 업데이트와 상태를 보내줌, 사용자 정의 데이터 필드
            int player2Score = (int)player2.CustomProperties["Score"];
            return player2Score.CompareTo(player1Score); // 현재 객체와 비교할 다른 객체의 값을 비교하여 세 가지 가능한 결과 중 하나를 정수로 반환
                                                         // if(player2의 점수 > player1의 점수){ CompareTo 메소드: 양수를 반환}
                                                         // 이는 player2가 player1보다 리스트에서 더 앞에 위치해야 함을 의미. 즉, 점수가 높은 플레이어가 리스트의 앞쪽으로 이동
        });
        


        // UI에 표시할 최대 플레이어 수. 최대 5명으로 설정
        int playerCount = Math.Min(playerList.Count, 5); // 두 개의 숫자 중 더 작은 숫자를 반환

        foreach (UI_PlayerRankingSlot slot in Slots)
        {
            slot.gameObject.SetActive(false); // UI 슬롯을 모두 비활성화
        }
        
        for (int i = 0; i < playerCount; i++)
        {
            Slots[i].gameObject.SetActive(true); // 활성화할 플레이어 수만큼 UI 슬롯을 활성화, 플레이어 정보를 설정
            Slots[i].Set(playerList[i]);
        }

        MySlot.Set(PhotonNetwork.LocalPlayer); // 현재 플레이어(자신)의 정보를 별도의 UI 슬롯에 설정
    }

}
