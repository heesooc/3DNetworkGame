using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterAbility : MonoBehaviour // abstract: 추상 클래스 
{
   protected Character Owner { get; private set; }

    private void Awake()
    {
        Owner = GetComponentInParent<Character>();
    }
}
