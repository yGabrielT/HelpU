using Player.Input;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using Cinemachine;
using PlayerInputManager = Player.Input.PlayerInputManager;
namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Camera/Mouse")]
        [SerializeField] private GameObject _cam;
        [SerializeField] private GameObject _interactUI;
        [SerializeField] private float _sensivity;
        [SerializeField] private float _zRotateControl = 4f;
        [SerializeField] private float _zRotateSmooth = 10f;
        [SerializeField] private float _smoothLerpMouseValue;
        [SerializeField] private float _topClampValue;
        [SerializeField] private float _bottomClampValue;
        [SerializeField] private bool _isCursorLocked = true;

        [Header("Noise values")]
        [SerializeField] private float _walkNoiseFrequency = 1f;
        [SerializeField] private float _walkNoiseAmplitude = 1f;
        [SerializeField] private float _idleNoiseFrequency = 0.3f;
        [SerializeField] private float _idleNoiseAmplitude = 0.3f;
        [SerializeField] private float _smoothLerpNoiseValue = 30f;

        [Header("Player Settings")]
        [SerializeField] private float _speed;
        [SerializeField] private float _smoothLerpMoveValue = 400f;
        [SerializeField] private float _gravity = -10f;
        [SerializeField] private float _interactRange = 20f;
        [SerializeField] private float _wallRange = 2f;
        [SerializeField] private LayerMask _layerToIgnoreRaycast;
        [SerializeField] private LayerMask _layerToWall;
        [SerializeField] private float _jumpPower = 4f;
        [SerializeField] private float _jumpMaxTime = 2f;
        [SerializeField] private float _speedMidAir;
        [SerializeField] private float _yInverseSpeed = .4f;
        [SerializeField] private float _maxYSpeed;
        [SerializeField] private float _minYSpeed;


        [HideInInspector] public bool isHanging;
        [HideInInspector] public PlayerInputManager playerInput;

        private Vector2 _rawMouseVector;
        private Vector2 _smoothMouseVector;
        private Vector2 _finalMouseVector;
        private Vector2 _rawMoveVector;
        private Vector3 _relativeMoveVector;
        private Vector2 _smoothMoveVector;
        private Vector3 _finalMoveVector;
        private float _jumpTime;
        private bool _isInAir;
        private CharacterController _char;
        private float _jumpValue;
        private float _verticalSpeed;
        private float _startYCam;
        private bool _isHolding;
        private float _rotateZ;
        private CinemachineVirtualCamera _vCam;
        private float _actualFrequency;
        private float _actualAmplitude;
        private GameObject lastPebble;
        private bool _isInPeeble;
        

        void Start()
        {
            _vCam = _cam.GetComponentInChildren<CinemachineVirtualCamera>();
            _startYCam = _cam.transform.localPosition.y;
            if (_isCursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            playerInput = GetComponent<PlayerInputManager>();

            _char = GetComponent<CharacterController>();
        }


        void Update()
        {
            Rotate();
            Interact();
            HandleJump();
            if (!isHanging)
            {
                MovePlayer();
            }
            Slope();
            HandleNoise();
            if(lastPebble != null)
            {
                if (isHanging && !_isInPeeble)
                {
                    _isInPeeble = true;
                    lastPebble.GetComponent<Pebble>()._collider.enabled = false;
                }
                else if (_isInPeeble && _isInAir)
                {
                    lastPebble.GetComponent<Pebble>().TriggerRedo();
                    _isInPeeble = false;
                    
                    
                }
            }
        }

        void HandleNoise()
        {

            //walk
            if(_rawMoveVector != Vector2.zero && !_isHolding && !isHanging)
            {
                _actualAmplitude = Mathf.Lerp(_actualAmplitude, _walkNoiseAmplitude, Time.deltaTime * _smoothLerpNoiseValue);
                _actualFrequency = Mathf.Lerp(_actualFrequency, _walkNoiseAmplitude, Time.deltaTime * _smoothLerpNoiseValue);
            }
            //idle
            else if ((_rawMoveVector == Vector2.zero || !_char.isGrounded) && !isHanging)
            {
                _actualAmplitude = Mathf.Lerp(_actualAmplitude, _idleNoiseAmplitude, Time.deltaTime * _smoothLerpNoiseValue);
                _actualFrequency = Mathf.Lerp(_actualFrequency, _idleNoiseAmplitude, Time.deltaTime * _smoothLerpNoiseValue);
            }
            //Hanging
            if (isHanging)
            {
                _actualAmplitude = Mathf.Lerp(_actualAmplitude, .1f, Time.deltaTime * _smoothLerpNoiseValue);
                _actualFrequency = Mathf.Lerp(_actualFrequency, .1f, Time.deltaTime * _smoothLerpNoiseValue);
            }

            _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = _actualAmplitude;
            _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = _actualFrequency;

        }

        void Rotate()
        {
            
            _rawMouseVector = new Vector2(playerInput.mouseDelta.x * _sensivity, playerInput.mouseDelta.y * _sensivity);
            _smoothMouseVector = Vector2.Lerp(_smoothMouseVector, _rawMouseVector, Time.deltaTime * _smoothLerpMouseValue);
            _finalMouseVector += _smoothMouseVector;
            _finalMouseVector.y = Mathf.Clamp(_finalMouseVector.y, _topClampValue, _bottomClampValue);
            _rotateZ = Mathf.Lerp(_rotateZ, _zRotateControl * -playerInput.movement.x, _zRotateSmooth * Time.deltaTime);
            _cam.transform.rotation = Quaternion.Euler(-_finalMouseVector.y, _finalMouseVector.x, _rotateZ);
            
        }

        void MovePlayer()
        {
            Vector3 forward = _cam.transform.forward;
            Vector3 right = _cam.transform.right;
            forward.y = 0;
            right.y = 0;
            forward = forward.normalized;
            right = right.normalized;


            Vector3 relativeInputX = playerInput.movement.x * right;
            Vector3 relativeInputY = playerInput.movement.y * forward;


             
            _relativeMoveVector = relativeInputY + relativeInputX;

            if (_char.isGrounded)
            {
                _rawMoveVector = new Vector2(_relativeMoveVector.x, _relativeMoveVector.z) * _speed;
            }
            else
            {
                _rawMoveVector /= 2;
                _rawMoveVector += (new Vector2(_relativeMoveVector.x, _relativeMoveVector.z) / _speedMidAir);
            }
                
            



            _smoothMoveVector = Vector2.Lerp(_smoothMoveVector, _rawMoveVector, Time.deltaTime * _smoothLerpMoveValue);


            _verticalSpeed += _gravity * Time.deltaTime;
            if(_verticalSpeed  < 0 && _char.isGrounded) _verticalSpeed = 0;

            _verticalSpeed = Mathf.Clamp(_verticalSpeed, _minYSpeed, _maxYSpeed);
            _finalMoveVector = new Vector3(_smoothMoveVector.x, _verticalSpeed, _smoothMoveVector.y);
            
            _char.Move(_finalMoveVector);
            
           
            
        }

        void HandleJump()
        {
            
            if (isHanging || _char.isGrounded)
            {
                if (playerInput.jump && _jumpTime < _jumpMaxTime && !_isInAir)
                {
                    _isHolding = true;
                    _jumpTime += Time.deltaTime;
                    _cam.transform.localPosition = Vector3.Lerp(_cam.transform.localPosition, new Vector3(_cam.transform.localPosition.x, .5f, _cam.transform.localPosition.z), Time.deltaTime * 10);;
                }
            }
            if (playerInput.jumpCancel && _isHolding)
            {
                
                playerInput.jump = false;
                playerInput.jumpCancel = false;
                _isInAir = true;
                _jumpValue = (_jumpPower * _jumpTime);
                _verticalSpeed += _jumpValue * Time.deltaTime;
                if (isHanging)
                {
             
                    _rawMoveVector.y = .1f;
                }
                isHanging = false;
                _isHolding = false;
            }
            else
            {
                playerInput.jumpCancel = false;
            }
            if (_isInAir)
            {
                _cam.transform.localPosition = Vector3.Lerp(_cam.transform.localPosition, new Vector3(_cam.transform.localPosition.x, 1f, _cam.transform.localPosition.z), Time.deltaTime * 20);
            }

            if ((isHanging ||_char.isGrounded) && _isInAir)
            {
                _isHolding = false;
                _jumpValue = 0;
                _jumpTime = 0;
                _isInAir = false;
            }
            
            
            
        }


        void Interact()
        {
            RaycastHit hit;
            if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, _interactRange, ~_layerToIgnoreRaycast))
            {
                if (hit.collider.transform.gameObject.TryGetComponent<IInterectable>(out IInterectable interactableObj) && !interactableObj.wasInteracted)
                {
                    Debug.Log("Looking at interectable");
                    _interactUI.SetActive(true);

                    if (playerInput.leftClick)
                    {
                        Debug.Log(hit.collider.transform.gameObject.name + "was interacted");

                        interactableObj.wasInteracted = true;
                        interactableObj.Response();

                    }

                }
                else
                {
                    playerInput.leftClick = false;
                    _interactUI.SetActive(false);
                }

            }
            else
            {
                playerInput.leftClick = false;
                _interactUI.SetActive(false);
            }
        }

        void Slope()
        {
            var SphereVerticalOffset = _char.height / 2 - _char.radius;
            var castOrigin = transform.position - new Vector3(0, SphereVerticalOffset, 0);

            if (Physics.SphereCast(castOrigin, _char.radius - 0.01f, Vector3.down , out var sphereHit, .06f, ~LayerMask.GetMask("Player"),QueryTriggerInteraction.Ignore))
            {
                var collider = sphereHit.collider;
                var angle = Vector3.Angle(Vector3.up, sphereHit.normal);
                Debug.DrawLine(sphereHit.point,sphereHit.point + sphereHit.normal, Color.blue,3f);
                Debug.Log(angle);
                if (angle > _char.slopeLimit)
                {
                    var normal = sphereHit.normal;
                    var yInverse = _yInverseSpeed - normal.y;

                    _rawMoveVector.x += yInverse * normal.x;
                    _rawMoveVector.y += yInverse * normal.z;
                }

            }

        }


        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.TryGetComponent<Pebble>(out Pebble PebbleObj))
            {
                lastPebble = other.gameObject;
                
            }
        }


    }
}