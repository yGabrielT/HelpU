using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Input
{
    public class PlayerInputManager : MonoBehaviour
    {
        public PlayerInputAction _playerActions;

        public bool leftClick;
        public Vector2 mouseDelta;
        public Vector2 movement;
        public bool jump;
        public bool jumpCancel;
        public bool restart;
        public bool pause;
        void Awake()
        {
            
            _playerActions = new PlayerInputAction();
            _playerActions.Player.Enable();
            _playerActions.Player.MouseDelta.performed += MouseDelta;
            _playerActions.Player.Movement.performed += Movement;
            _playerActions.Player.LeftClick.performed += LeftClick;
            _playerActions.Player.Jump.started += StartJump;
            _playerActions.Player.Jump.canceled += CancelJump;
            _playerActions.Player.Restart.performed += Restart;
            _playerActions.Player.Restart.canceled += CancelRestart;
            _playerActions.Player.Pause.performed += Pause;
        }

    
        void Update()
        {

        }

        void MouseDelta(InputAction.CallbackContext context)
        {
            mouseDelta = _playerActions.Player.MouseDelta.ReadValue<Vector2>();
        }

        void Movement(InputAction.CallbackContext context)
        {
            movement = _playerActions.Player.Movement.ReadValue<Vector2>();
        }

        void LeftClick(InputAction.CallbackContext context)
        {
            leftClick = true;
        }

        void Pause(InputAction.CallbackContext context)
        {
            pause = true;
        }
        void Restart(InputAction.CallbackContext context)
        {
            restart = true;
        }

        void CancelRestart(InputAction.CallbackContext context)
        {
            restart = false;
        }

        void StartJump(InputAction.CallbackContext context)
        {
            jump = true;
        }

        void CancelJump(InputAction.CallbackContext context)
        {
            jumpCancel = true;
        }
    }
}

