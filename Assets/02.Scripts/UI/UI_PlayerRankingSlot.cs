using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SerializeField]
public class UI_PlayerRankingSlot : MonoBehaviour
{
    public Text RankingTextUI;
    public Text NicknameTextUI;
    public Text KillCountTextUI;
    public Text ScoreTextUI;

    public void Set(Player player)
    {
        RankingTextUI.text = "-";
        NicknameTextUI.text = player.NickName;
        if(player.CustomProperties.ContainsKey("KillCount"))
        {
            KillCountTextUI.text = $"{player.CustomProperties["KillCount"]}";
            ScoreTextUI.text = $"{player.CustomProperties["Score"]}";
        }
        else
        {
            KillCountTextUI.text = "0";
            ScoreTextUI.text = "0";
        }
    }
}
