using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterPlayer : MonoBehaviour
{

    [SerializeField] private UnityEvent _event;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == ("Player"))
        {
            _event.Invoke();
        }
    }
}
