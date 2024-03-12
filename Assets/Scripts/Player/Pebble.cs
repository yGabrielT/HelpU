using DG.Tweening;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pebble : MonoBehaviour, IInterectable
{
    public bool wasInteracted { get; set; }
    private PlayerController _playerController;
    [HideInInspector] public BoxCollider _collider;
    [SerializeField] private Transform _whereToMove;
    public void Response()
    {
        
        if (_playerController.canClimb && wasInteracted)
        {
            _playerController.isHanging = true;
            _playerController.transform.DOMove(_whereToMove.position, .4f).SetEase(Ease.OutCirc)
                .OnComplete(() =>
                {
                    AudioManager.instance.PlayOneShotAtPosRandPitch(_playerController.grabPeebleSound, transform, .6f, 1.3f);
                    Instantiate(_playerController.smokeParticle.gameObject, transform.position , Quaternion.identity);
                });
        }
        Invoke(nameof(restoreInteract), .5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _collider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void restoreInteract()
    {
        wasInteracted = false;
    }

}
