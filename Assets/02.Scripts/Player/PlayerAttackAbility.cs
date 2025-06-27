using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackAbility : PlayerAbility
{    
    public float AttackStaminaCost = 20f;
    private Animator _animator;

    private float _attackTimer = 0f;
    public bool IsAttack { get; private set; } = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        Timer();
    }

    private void Timer()
    {
        if(!_photonView.IsMine) return;

        _attackTimer += Time.deltaTime;
    }
    private void AttackEnd()
    {
        IsAttack = false;
    }
    public void Attack(InputAction.CallbackContext context)
    {
        if(context.ReadValue<float>() > 0 && _attackTimer >= (1f / _owner.Stat.AttackSpeed)
        && _owner.Stat.Stamina > AttackStaminaCost)
        {
            _owner.GetAbility<PlayerStamina>().UseStamina(AttackStaminaCost);
            _attackTimer = 0f;
            IsAttack = true;
            _animator.SetTrigger($"Attack{Random.Range(1, 4)}");
        }
    }
}
