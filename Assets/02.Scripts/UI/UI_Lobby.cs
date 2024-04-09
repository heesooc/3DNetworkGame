using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterType
{
    Female,
    Male,
}

public class UI_Lobby : MonoBehaviour
{
    public static CharacterType SelectedCharacterType = CharacterType.Female;

    public GameObject FemaleCharacter;
    public GameObject MaleCharacter;

    public InputField NicknameInputFieldUI;
    public InputField RoomIDInputFieldUI;

    private void Start()
    {
        FemaleCharacter.SetActive(SelectedCharacterType == CharacterType.Female);
        MaleCharacter.SetActive(SelectedCharacterType == CharacterType.Male);
    }

    // 방만들기 버튼을 눌렀을 때 호출되는 함수
    public void OnClickMakeRoomButton()
    {
        string nickname = NicknameInputFieldUI.text;
        string roomID = RoomIDInputFieldUI.text;

        if(string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(roomID))
        {
            Debug.Log("입력하세요.");
            return;
        }

        PhotonNetwork.NickName = nickname;

        RoomOptions roomOptions = new RoomOptions();
         roomOptions.MaxPlayers = 20;    // 입장 가능한 최대 플레이어 수
         roomOptions.IsVisible = true;   // 로비에서 방 목록에 노출할 것인가?
         roomOptions.IsOpen = true;      // 방에 다른 플레이어가 들어올 수 있는가?
        roomOptions.EmptyRoomTtl = 1000 * 20; // 비어 있는 방이 얼마나 타임 투 리브(Time to live, TTL): 컴퓨터나 네트워크에서 데이터의 유효 기간을 나타내기 위한 방법

        PhotonNetwork.JoinOrCreateRoom(roomID, roomOptions, TypedLobby.Default); // 방이 있다면 입장하고 없다면 만드는 것
    }

    // 캐릭터 타입 버튼을 눌렀을 때 호출되는 함수
    public void OnClickMaleButton() => OnClickCharacterTypeButton(CharacterType.Male);
    public void OnClickFemaleButton() => OnClickCharacterTypeButton(CharacterType.Female);

    private void OnClickCharacterTypeButton(CharacterType characterType)
    {
        SelectedCharacterType = characterType;
        FemaleCharacter.SetActive(SelectedCharacterType == CharacterType.Female);
        MaleCharacter.SetActive(SelectedCharacterType == CharacterType.Male);
    }
}
