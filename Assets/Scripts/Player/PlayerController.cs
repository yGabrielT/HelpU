using Player.Input;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Cinemachine;
using PlayerInputManager = Player.Input.PlayerInputManager;
using UnityEngine.UI;
using TMPro;
namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Camera/Mouse")]
        [SerializeField] private GameObject _cam;
        [SerializeField] private GameObject _interactUI;
        [SerializeField] public float sensivity;
        [SerializeField] private float _zRotateControl;
        [SerializeField] private float _zRotateSmooth;
        [SerializeField] private float _smoothLerpMouseValue;
        [SerializeField] private float _topClampValue;
        [SerializeField] private float _bottomClampValue;

        [Header("Noise values")]
        [SerializeField] private float _walkNoiseFrequency;
        [SerializeField] private float _walkNoiseAmplitude;
        [SerializeField] private float _idleNoiseFrequency;
        [SerializeField] private float _idleNoiseAmplitude;
        [SerializeField] private float _smoothLerpNoiseValue;

        [Header("Player Settings")]
        [SerializeField] private float _speed;
        [SerializeField] private float _smoothLerpMoveValue;
        [SerializeField] private float _gravity;
        [SerializeField] private float _interactRange;
        [SerializeField] private float _wallRange;
        [SerializeField] private LayerMask _layerToIgnoreRaycast;
        [SerializeField] private LayerMask _layerToWall;
        [SerializeField] private float _jumpPower;
        [SerializeField] private float _jumpMaxTime;
        [SerializeField] private float _speedMidAir;
        [SerializeField] private float _yInverseSpeed;
        [SerializeField] private float _maxYSpeed;
        [SerializeField] private float _minYSpeed;
        [SerializeField] public float _maxStamina;
        [SerializeField] public float _maxGroundTimer;
        [SerializeField] float _maxCheckTimer = 10;
        [SerializeField] Image _panelImage;
        [HideInInspector] public bool isHanging;
        [HideInInspector] public PlayerInputManager playerInput;
        public bool canControl = true;

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
        private bool _isHolding;
        private float _rotateZ;
        private CinemachineVirtualCamera _vCam;
        private float _actualFrequency;
        private float _actualAmplitude;
        [HideInInspector] public float _stamina;
        [HideInInspector] public bool canClimb;
        private bool _canRestoreStamina;
        public float _groundTimer;
        private bool _isGroundTimerDone;
        private bool isSliding;
        public Vector3 _lastCheckPoint;
        bool canCheckPoint = true;
        private float timerChecker;
        [SerializeField] private TextMeshProUGUI _checkpointText;
        [SerializeField]
        private AudioClip _footStepClip;
        [SerializeField]
        private float _stepDelay = .2f;
        private float _stepTimer;
        public AudioClip grabPeebleSound;
        
        public GameObject smokeParticle;
        public bool canRestart;
        
        public bool isInCutscene;

        void Start()
        {
            
            canCheckPoint = true;
            _stamina = _maxStamina;
            canClimb = true;
            _vCam = _cam.GetComponentInChildren<CinemachineVirtualCamera>();
            playerInput = GetComponent<PlayerInputManager>();

            _char = GetComponent<CharacterController>();
        }


        void Update()
        {
            
            if (canControl)
            {
                HandleCheckpoint();
                Rotate();
                Interact();
                HandleJump();
                if (!isHanging)
                {
                    MovePlayer();
                }
                Slope();
                HandleNoise();
                Stamina();
                ChooseRandomFootStep();
            }
            
            
            
        }

        
        void ChooseRandomFootStep()
        {
            if(_char.isGrounded && _rawMoveVector != Vector2.zero && !isHanging)
            {
                if(_stepTimer > 0)
                {
                    _stepTimer -= Time.deltaTime;
                }
                else
                {
                    _stepTimer = _stepDelay;
                    
                    AudioManager.instance.PlayOneShotAtPosRandPitch(_footStepClip, this.transform,0.1f,1.5f);

                }
            }
        }

        void Stamina()
        {
            
            if (isHanging)
            {
                _stamina -= Time.deltaTime;
                _canRestoreStamina = false;
                _smoothMoveVector = Vector2.zero;
                _finalMoveVector = Vector2.zero;
            }
            else
            {
                _canRestoreStamina = true;
            }
            if(_stamina <= 0f)
            {
                isHanging = false;
                canClimb = false;
                
            }
            

            if (_char.isGrounded && _canRestoreStamina && !_isGroundTimerDone && !isSliding)
            {
                _groundTimer += Time.deltaTime;
                if (_groundTimer > _maxGroundTimer)
                {
                    _isGroundTimerDone = true;
                }
            }
            
            
            
            if(_stamina < _maxStamina)
            {
                if (_canRestoreStamina && _groundTimer > _maxGroundTimer && _isGroundTimerDone)
                {
                    _stamina += Time.deltaTime * (_maxStamina/2);
                }
            }
            else
            {
                _groundTimer = 0f;
                _isGroundTimerDone = false;
                _canRestoreStamina = false;
                canClimb = true;
            }
            
            
        }
        void HandleNoise()
        {

            //walk
            if(_rawMoveVector != Vector2.zero && !_isHolding && !isHanging)
            {
                _actualAmplitude = Mathf.Lerp(_actualAmplitude, _walkNoiseAmplitude, Time.deltaTime * _smoothLerpNoiseValue);
                _actualFrequency = Mathf.Lerp(_actualFrequency, _walkNoiseFrequency, Time.deltaTime * _smoothLerpNoiseValue);
            }
            //idle
            else if ((_rawMoveVector == Vector2.zero || !_char.isGrounded) && !isHanging)
            {
                _actualAmplitude = Mathf.Lerp(_actualAmplitude, _idleNoiseAmplitude, Time.deltaTime * _smoothLerpNoiseValue);
                _actualFrequency = Mathf.Lerp(_actualFrequency, _idleNoiseFrequency, Time.deltaTime * _smoothLerpNoiseValue);
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
            
            _rawMouseVector = new Vector2(playerInput.mouseDelta.x * sensivity, playerInput.mouseDelta.y * sensivity);
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


            if (!isSliding)
            {
                _relativeMoveVector = relativeInputY + relativeInputX;
            }
            else
            {
                _relativeMoveVector = (relativeInputY + relativeInputX)/2;
            }
                


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
            if (_verticalSpeed < 0 && _char.isGrounded)
            {
                _verticalSpeed = 0;
                
            }
            _verticalSpeed = Mathf.Clamp(_verticalSpeed, _minYSpeed, _maxYSpeed);
            _finalMoveVector = new Vector3(_smoothMoveVector.x, _verticalSpeed, _smoothMoveVector.y);
            
            _char.Move(_finalMoveVector);
            
           
            
        }

        void HandleJump()
        {
            
            if (isHanging || (_char.isGrounded && !isSliding))
            {
                if (playerInput.jump && _jumpTime < _jumpMaxTime && !_isInAir)
                {
                    _isHolding = true;
                    _jumpTime += Time.deltaTime;
                    _cam.transform.localPosition = Vector3.Lerp(_cam.transform.localPosition, new Vector3(_cam.transform.localPosition.x, .5f, _cam.transform.localPosition.z), Time.deltaTime * 10);
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
                AudioManager.instance.PlayOneShotAtPos1(_footStepClip, transform, 0.1f);
            }
            
            
            
        }


        void Interact()
        {
            RaycastHit hit;
            if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out hit, _interactRange, ~_layerToIgnoreRaycast))
            {
                if (hit.collider.transform.gameObject.TryGetComponent<IInterectable>(out IInterectable interactableObj) && !interactableObj.wasInteracted)
                {
                    
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

                
                if (angle > _char.slopeLimit)
                {
                    isSliding = true;
                    var normal = sphereHit.normal;
                    var yInverse = _yInverseSpeed - normal.y;

                    _rawMoveVector.x += yInverse * normal.x;
                    _rawMoveVector.y += yInverse * normal.z;
                }
                else
                {
                    isSliding = false;
                }

            }
            

        }

        public void StoreCheckpoint(Transform local)
        {
            _lastCheckPoint = local.position;
            _checkpointText.rectTransform.anchoredPosition = new Vector2(0, -50f);
            _checkpointText.rectTransform.DOAnchorPos(new Vector2(0, 100f), 2f).SetEase(Ease.OutExpo);
            _checkpointText.DOFade(.5f, .4f).OnComplete(() => _checkpointText.DOFade(0f, .4f).SetDelay(1.2f));
        }

        public void HandleCheckpoint()
        {
            if (canRestart)
            {
                if (timerChecker > 0)
                {
                    timerChecker -= Time.deltaTime;

                }
                else
                {
                    canCheckPoint = true;
                }

                if (_lastCheckPoint != null && canCheckPoint && playerInput.restart)
                {


                    GoToCheckpoint();



                }
            }
            
        }

        public void GoToCheckpoint()
        {
            canRestart = true;
            timerChecker = _maxCheckTimer;
            canCheckPoint = false;
            playerInput.restart = false;

            _panelImage.DOFade(1f, .5f).SetEase(Ease.InOutCirc).OnComplete(() =>
            {
                _char.enabled = false;
                transform.position = _lastCheckPoint;
                _char.enabled = true;
                _panelImage.DOFade(0f, .5f).SetEase(Ease.InOutCirc);
            }
            );
        }

        

        public void ManageControl(bool isControlling)
        {
            canControl = isControlling;
            isInCutscene = !isControlling;
        }


    }
}