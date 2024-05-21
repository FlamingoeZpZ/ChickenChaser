using Characters;
using UnityEngine;

namespace Managers
{
    public static class PlayerControls
    {
        private static InputControls _controls;
        private static Chicken p;
        public static void Init(Chicken player)
        {
            p = player;
            _controls = new InputControls();

            _controls.Game.EnableUI.performed += _ => EnableUI();
            
            _controls.Game.Ability.performed += ctx => player.ChangeAbilityState(ctx.ReadValueAsButton()); // Cooldowns
            _controls.Game.Cluck.performed += ctx => player.ChangeCluckState(ctx.ReadValueAsButton()); // Cooldowns
            _controls.Game.Look.performed += ctx => player.Look(ctx.ReadValue<Vector2>()); // Cooldowns
            _controls.Game.Move.performed += ctx => player.Move(ctx.ReadValue<Vector2>()); // Cooldowns
            
            _controls.UI.DisableUI.performed += _ => DisableUI();
        }

        public static void EnableUI()
        {
            _controls.Game.Disable();
            _controls.UI.Enable();
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public static void DisableUI()
        {
            _controls.UI.Disable();
            _controls.Game.Enable();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private static void DisablePlayer()
        {
            p.Look(Vector2.zero);
            p.Move(Vector2.zero);
            p.ChangeAbilityState(false);
            p.ChangeCluckState(false);
        }
    }
}
