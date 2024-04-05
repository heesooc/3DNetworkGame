using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MinimapMarker : MonoBehaviour
{
    public Image markerImage; // Inspector에서 설정해야 하는 마커 이미지
    public Sprite blueArrow; // 자신의 마커 이미지
    public Sprite redArrow; // 다른 플레이어의 마커 이미지

    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        // UI Profile 이미지를 찾습니다. 'UIProfile'는 UI 요소의 이름이어야 합니다.
        markerImage = GetComponentInChildren<Image>();
        SetupMarkerImage();
    }

    private void SetupMarkerImage()
    {
        if (photonView.IsMine)
        {
            // 이 플레이어가 로컬 플레이어인 경우
            markerImage.sprite = blueArrow;
        }
        else
        {
            // 이 플레이어가 원격 플레이어인 경우
            markerImage.sprite = redArrow;
        }
    }
}
