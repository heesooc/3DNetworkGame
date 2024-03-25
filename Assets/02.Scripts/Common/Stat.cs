using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // 데이터 직렬화, MonoBehaviour 안씀

public class Stat 
{
    public int Health;
    public float MoveSpeed;
    public float RotationSpeed;
    public float AttackCoolTime;
}
