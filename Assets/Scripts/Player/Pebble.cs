using DG.Tweening;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pebble : MonoBehaviour, IInterectable
{
    public bool wasInteracted { get; set; }
    private PlayerController _playerController;
    public void Response()
    {
        wasInteracted = true;
        _playerController.isHanging = true;
        _playerController.transform.DOMove(transform.position,.4f).SetEase(Ease.InOutBounce);
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
