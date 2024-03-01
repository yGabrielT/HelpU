using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Input
{
    public class PlayerInputManager : MonoBehaviour
    {
        private PlayerInputAction _playerActions;

        public bool leftClick;
        public Vector2 mouseDelta;
        public Vector2 movement;
        void Awake()
        {
            _playerActions = new PlayerInputAction();
            _playerActions.Player.Enable();
            _playerActions.Player.MouseDelta.performed += MouseDelta;
            _playerActions.Player.Movement.performed += Movement;
            _playerActions.Player.LeftClick.performed += LeftClick;
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
    }
}
