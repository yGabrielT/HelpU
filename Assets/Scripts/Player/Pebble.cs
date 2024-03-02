using DG.Tweening;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pebble : MonoBehaviour, IInterectable
{
    public bool wasInteracted { get; set; }
    private PlayerController _playerController;
    [HideInInspector]public BoxCollider _collider;
    public void Response()
    {
        wasInteracted = false;
        _playerController.isHanging = true;
        _playerController.transform.DOMove(transform.position - new Vector3(0,1f, 0),.4f).SetEase(Ease.OutCirc);
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

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == ("Player"))
        {
            
            Response();
            
        }
    }

    public void TriggerRedo()
    {
        Invoke(nameof(RedoCollider),1f);
    }
    public void RedoCollider()
    {
        _collider.enabled = true;
    }
}
