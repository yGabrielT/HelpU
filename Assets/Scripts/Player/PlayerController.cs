using Player.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Input;
namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public GameObject cam;
        public GameObject interactUI;
        public float sensivity;
        public float speed;
        public float smoothLerpMouseValue;
        public float smoothLerpMoveValue = 400f;
        public float topClampValue;
        public float bottomClampValue;
        public float gravity = -10f;
        public bool isCursorLocked = true;
        public float interactRange = 20f;
        public float wallRange = 2f;
        public LayerMask layerToIgnoreRaycast;
        public LayerMask layerToWall;


        //Private
        private Vector2 _rawMouseVector;
        private Vector2 _smoothMouseVector;
        private Vector2 _finalMouseVector;
        private Vector2 _rawMoveVector;
        private Vector3 _relativeMoveVector;
        private Vector2 _smoothMoveVector;
        private Vector3 _finalMoveVector;
        public bool isHanging;

        [HideInInspector] public PlayerInputManager inputManger;
        private CharacterController _char;


        void Start()
        {
            if (isCursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            inputManger = GetComponent<PlayerInputManager>();

            _char = GetComponent<CharacterController>();
        }


        void Update()
        {
            Rotate();
            Interact();
            ClimbWhenClose();
            
            if (!isHanging)
            {
                MovePlayer();
            }

        }



        void Rotate()
        {
            
            _rawMouseVector = new Vector2(inputManger.mouseDelta.x * sensivity, inputManger.mouseDelta.y * sensivity);
            _smoothMouseVector = Vector2.Lerp(_smoothMouseVector, _rawMouseVector, Time.deltaTime * smoothLerpMouseValue);
            _finalMouseVector += _smoothMouseVector;
            _finalMouseVector.y = Mathf.Clamp(_finalMouseVector.y, topClampValue, bottomClampValue);
            cam.transform.rotation = Quaternion.Euler(-_finalMouseVector.y, _finalMouseVector.x, transform.rotation.eulerAngles.z);
        }

        void MovePlayer()
        {
            Vector3 forward = cam.transform.forward;
            Vector3 right = cam.transform.right;
            forward.y = 0;
            right.y = 0;
            forward = forward.normalized;
            right = right.normalized;


            Vector3 relativeInputX = inputManger.movement.x * right;
            Vector3 relativeInputY = inputManger.movement.y * forward;

            _relativeMoveVector = relativeInputY + relativeInputX;

            _rawMoveVector = new Vector2(_relativeMoveVector.x * speed, _relativeMoveVector.z * speed);




            _smoothMoveVector = Vector2.Lerp(_smoothMoveVector, _rawMoveVector, Time.deltaTime * smoothLerpMoveValue);


            
            _finalMoveVector = new Vector3(_smoothMoveVector.x, gravity * Time.deltaTime, _smoothMoveVector.y);
            _char.Move(_finalMoveVector);
            
           
            
        }

        void Interact()
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, interactRange, ~layerToIgnoreRaycast))
            {
                if (hit.collider.transform.gameObject.TryGetComponent<IInterectable>(out IInterectable interactableObj) && !interactableObj.wasInteracted)
                {
                    Debug.Log("Looking at interectable");
                    interactUI.SetActive(true);

                    if (inputManger.leftClick)
                    {
                        Debug.Log(hit.collider.transform.gameObject.name + "was interacted");

                        interactableObj.wasInteracted = true;
                        interactableObj.Response();

                    }

                }
                else
                {
                    inputManger.leftClick = false;
                    interactUI.SetActive(false);
                }

            }
            else
            {
                inputManger.leftClick = false;
                interactUI.SetActive(false);
            }
        }

        void ClimbWhenClose()
        {
            
        }


    }
}