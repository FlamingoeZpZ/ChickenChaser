using Characters;
using UI;
using UnityEngine;

namespace Managers
{
    public static class PlayerControls
    {
        private static InputControls _controls;
        private static PlayerChicken p;
        
        // Initialize the controls
        public static void Init(PlayerChicken player)
        {
            //Remember our player
            p = player;
            _controls = new InputControls();
            
            //Managed Abilities
            _controls.Game.Ability.performed += ctx => player.ChangeAbilityState(ctx.ReadValueAsButton()); 
            _controls.Game.Cluck.performed += ctx => player.ChangeCluckState(ctx.ReadValueAsButton());
            _controls.Game.Jump.performed += ctx => player.ChangeJumpState(ctx.ReadValueAsButton()); 

            //Bind other player actions
            _controls.Game.Look.performed += ctx => player.Look(ctx.ReadValue<Vector2>()); 
            _controls.Game.Move.performed += ctx => player.Move(ctx.ReadValue<Vector2>()); 
            
            //Controls for toggling UI
            _controls.Game.EnableUI.performed += _ =>
            {
                Settings.OpenSettings(false);
                EnableUI();
            };
            
            _controls.UI.DisableUI.performed += _ =>
            {
                Settings.CloseSettings();
                DisableUI();
            };
        }

        public static void EnableUI()
        {
            DisablePlayer();
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

        public static void DisablePlayer()
        {
            _controls.UI.Disable();
            _controls.Game.Disable();
            p.Look(Vector2.zero);
            p.Move(Vector2.zero);
            p.ChangeAbilityState(false);
            p.ChangeCluckState(false);
            p.ChangeJumpState(false);

        }
    }
}
