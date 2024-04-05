using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CampFire : MonoBehaviour
{
    public int Damage       = 20;
    public float Cooltime   = 0.6f;
    public float _timer    = 0f; // 타이머도 안올라감

    private IDamaged _target = null;

    private void OnTriggerEnter(Collider other)
    {
        IDamaged damagedObject = other.GetComponent<IDamaged>();
        if (damagedObject == null)
        {
            return;
        }

        PhotonView photonView = other.GetComponent<PhotonView>();
        if (photonView == null || !photonView.IsMine)
        {
            return;
        }

        _target = damagedObject;

        _target.Damaged(Damage, -1);
    }

    private void OnTriggerStay(Collider other)
    {
        Character c = other.GetComponent<Character>();
        if (_target != null && c != null && c.State == State.Death)
        {
            _target = null;
        }
        // 데미지 자꾸 먹는 오류 해결함

        if (_target == null) 
        { 
            return; 
        }

        if (other.TryGetComponent<Character>(out Character character)) //존재 여부 확인, 변수 직접 할당
        {
            if (character.State == State.Death)
            {
                _target = null;
                return;
            }
        }

        _timer += Time.deltaTime;
        if (_timer > Cooltime)
        {
            _timer = 0f;
            _target.Damaged(Damage, -1);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        IDamaged damagedObject = other.GetComponent<IDamaged>();
        if (damagedObject == null)
        {
            return;
        }
        if(damagedObject == _target)
        {
            _target = null;
            _timer = 0f;
        }
    }
}
