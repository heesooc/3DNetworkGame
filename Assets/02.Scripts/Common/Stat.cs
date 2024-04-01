using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

[Serializable] // 데이터 직렬화, MonoBehaviour 안씀

public class Stat 
{
    public int Health;
    public int MaxHealth;
    public int Damage;

    public float Stamina;
    public float MaxStamina;
    public float RunConsumeStamina;
    public float RecoveryStamina;

    public float MoveSpeed;
    public float RunSpeed;

    public float RotationSpeed;

    public float AttackCoolTime;
    public float AttackConsumeStamina;

    public float JumpPower;
    public int JumpMaxCount;
    public int JumpRemainCount;
    public float JumpConsumeStamina;

    public void Init()
    {
        Health = MaxHealth;
        Stamina = MaxStamina;
    }
}
