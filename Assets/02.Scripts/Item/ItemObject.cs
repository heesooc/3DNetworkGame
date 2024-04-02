using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Collider))]

public class ItemObject : MonoBehaviourPun // Pun씀
{
    [Header("아이템 타입")]
    public ItemType ItemType;
    public float Value = 100;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Character character = other.GetComponent<Character>();
            if(!character.PhotonView.IsMine || character.State == State.Death)
            {
                return;
            }
                        
            switch(ItemType)
            {
                case ItemType.HealthPotion:
                {
                        character.Stat.Health += (int)Value;
                        if(character.Stat.Health > character.Stat.MaxHealth)
                        {
                            character.Stat.Health = character.Stat.MaxHealth;
                        }

                        break;
                }
                case ItemType.StaminaPotion:
                {
                        character.Stat.Stamina += Value;
                        if (character.Stat.Stamina > character.Stat.MaxStamina)
                        {
                            character.Stat.Stamina = character.Stat.MaxStamina;
                        }
                        break;
                }
            }
            gameObject.SetActive(false); // 시간차가 있어서 써줌
            ItemObjectFactory.Instance.RequestDelete(photonView.ViewID);
        }
    }
}
