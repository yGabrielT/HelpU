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
        wasInteracted = false;
        if (_playerController.canClimb)
        {
            _playerController.isHanging = true;
            _playerController.transform.DOMove(_whereToMove.position, .4f).SetEase(Ease.OutCirc);
        }
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

}
