using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotionAbility : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            
        }
    }

    [PunRPC]
    private void PlayMotion(int number)
    {
        
    }
}
