using UnityEngine;
using UnityEngine.InputSystem;

public class InputFixer : MonoBehaviour
{
    private PlayerInput playerInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
         // 🎮 Gamepad 입력 감지 → Console 스킴
        if (playerInput.defaultControlScheme == "Console" && playerInput.currentControlScheme != "Console")
        {
            playerInput.SwitchCurrentControlScheme("Console", Gamepad.current);
            Debug.Log("🎮 Console 스킴 적용");
        }

        // ⌨ 마우스/키보드 입력 감지 → PC 스킴
        if (playerInput.defaultControlScheme == "PC" && playerInput.currentControlScheme != "PC")
        {
            playerInput.SwitchCurrentControlScheme("PC", Keyboard.current, Mouse.current);
            Debug.Log("⌨️ PC 스킴 적용");
        }

        // // 🎮 Gamepad 입력 감지 → Console 스킴
        // if (Gamepad.current != null && playerInput.currentControlScheme != "Console")
        // {
        //     if (Gamepad.current.leftStick.ReadValue() != Vector2.zero ||
        //         Gamepad.current.buttonSouth.wasPressedThisFrame)
        //     {
        //         playerInput.SwitchCurrentControlScheme("Console", Gamepad.current);
        //         Debug.Log("🎮 Gamepad 감지됨 - Console 스킴 적용");
        //         return;
        //     }
        // }

        // // ⌨ 마우스/키보드 입력 감지 → PC 스킴
        // if (playerInput.currentControlScheme != "PC")
        // {
        //     if (Keyboard.current.anyKey.wasPressedThisFrame ||
        //         Mouse.current.leftButton.wasPressedThisFrame ||
        //         Mouse.current.rightButton.wasPressedThisFrame ||
        //         Mouse.current.delta.ReadValue() != Vector2.zero)
        //     {
        //         playerInput.SwitchCurrentControlScheme("PC", Keyboard.current, Mouse.current);
        //         Debug.Log("⌨️ 키보드/마우스 감지됨 - PC 스킴 적용");
        //     }
        // }
    }
}
