using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;


public class CharacterCanvasAbility : CharacterAbility
{
    public Canvas MyCanvas;
    public Text NicknameTextUI;
    public Slider HealthSliderUI;
    public Slider StaminaSliderUI;


    private void Start()
    {
        NicknameTextUI.text = Owner.PhotonView.Controller.NickName; 
    }

    private void Update()
    {
        // todo. 빌보드 구현
        MyCanvas.transform.forward = Camera.main.transform.forward;

        HealthSliderUI.value = (float)Owner.Stat.Health / Owner.Stat.MaxHealth;
        StaminaSliderUI.value = Owner.Stat.Stamina / Owner.Stat.MaxStamina;
    }
}
