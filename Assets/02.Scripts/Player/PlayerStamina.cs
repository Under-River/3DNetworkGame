using System;
using UnityEngine;

public class PlayerStamina : PlayerAbility
{
    public Action _onStaminaChanged;
    private void Update()
    {
        RecoverStaminaPerSecond();
    }
    public void UseStamina(float stamina)
    {
        _owner.Stat.Stamina -= stamina;
        _onStaminaChanged?.Invoke();
    }
    public void UseStaminaPerSecond(float stamina)
    {
        _owner.Stat.Stamina -= stamina * Time.deltaTime;
        _onStaminaChanged?.Invoke();
    }
    public void RecoverStaminaPerSecond()
    {
        if(_owner.Stat.Stamina < _owner.Stat.MaxStamina 
        && !_owner.GetAbility<PlayerAttackAbility>().IsAttack
        && !_owner.GetAbility<PlayerMoveAbility>().IsSprinting)
        {
            _owner.Stat.Stamina += _owner.Stat.StaminaRecovery * Time.deltaTime;
            _onStaminaChanged?.Invoke();
        }
    }
}
