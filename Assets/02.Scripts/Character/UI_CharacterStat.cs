using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterStat : MonoBehaviour
{
    public Character MyCharacter;
    public Slider HealthSliderUI;
    public Slider StaminaSliderUI;

    private void Update()
    {
        if (MyCharacter == null)
        {
            return;
        }

        HealthSliderUI.value = (float)MyCharacter.Stat.Health / MyCharacter.Stat.MaxHealth;
        StaminaSliderUI.value = MyCharacter.Stat.Stamina / MyCharacter.Stat.MaxStamina;
    }
}
