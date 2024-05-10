using Characters;
using UnityEngine;

namespace Managers
{
    public static class PlayerControls
    {
        private static InputControls _controls;
    
        public static void Init(Chicken player)
        {
            _controls = new InputControls();

            _controls.Game.EnableUI.performed += _ => EnableUI();
            _controls.Game.Ability.performed += _ => player.TryAbility(); // Cooldowns
            _controls.Game.Look.performed += ctx => player.Look(ctx.ReadValue<Vector2>()); // Cooldowns
            _controls.Game.Move.performed += ctx => player.Move(ctx.ReadValue<Vector2>()); // Cooldowns
            _controls.UI.DisableUI.performed += _ => DisableUI();
        }

        public static void EnableUI()
        {
            _controls.Game.Disable();
            _controls.UI.Enable();

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        public static void DisableUI()
        {
            _controls.UI.Disable();
            _controls.Game.Enable();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
