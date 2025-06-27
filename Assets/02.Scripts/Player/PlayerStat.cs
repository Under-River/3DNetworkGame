using System;
using UnityEngine;

[Serializable]
public class PlayerStat
{
    public float MoveSpeed = 7f;
    public float SprintMultiplier = 1.5f;
    public float JumpPower = 2.5f;
    public float RotateSpeed = 200f;
    public float AttackSpeed = 1.2f; // 초당 1.2번 공격할 수 있다.
    public float MaxStamina = 100f;
    public float Stamina = 100f;
    public float StaminaRecovery = 20f;
}
